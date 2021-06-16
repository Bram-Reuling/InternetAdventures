using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Mirror;
using Networking;
using Shared;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Lobby;
using Shared.protocol.Match;
using Shared.protocol.protocol;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainServerClient : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private TMP_InputField _roomCodeInput;
    [SerializeField] private string _server = "";
    [SerializeField] private int _port = 55555;

    [Scene, SerializeField] private string _menuScene;
    [Scene, SerializeField] private string _gameScene;
    [Scene, SerializeField] private string _lobbyScene;

    [SerializeField] private bool UseLocalhost = false;

    private TcpClient _client;
    private bool _connectedToServer = false;

    private IPAddress ipAddress;

    private bool IsUsingIPAddress = false;

    [SerializeField] private string _clientName = "Bram";
    private int _clientId = 0;

    private string _joinedRoomCode = "";

    private void Awake()
    {
        if (DataHandler.IsMainServerClientAlreadySpawned == false)
        {
            DontDestroyOnLoad(this.gameObject);
            DataHandler.IsMainServerClientAlreadySpawned = true;
            DataHandler.MainServerClientInstance = this;
        }
        else
        {
            // Give data to already existing one
            DataHandler.MainServerClientInstance.SetInputFields(_nameInput, _descriptionInput, _roomCodeInput);
            DestroyImmediate(this.gameObject);
        }
    }

    public void SetInputFields(TMP_InputField name, TMP_InputField desc, TMP_InputField code)
    {
        _nameInput = name;
        _descriptionInput = desc;
        _roomCodeInput = code;
    }
    
    private void Start()
    {
        ipAddress = IPAddress.Parse(_server);
        //ConnectToServer();
        EventBroker.LoadedLobbyPanelEvent += LoadedLobbyPanel;
        EventBroker.ConnectToServerEvent += ConnectToServer;
        EventBroker.JoinLobbyEvent += JoinLobby;
        EventBroker.HostLobbyEvent += CreateLobby;
    }

    IEnumerator SendIsAlive()
    {
        while (_client.Connected)
        {
            try
            {
                Debug.Log("Trying to send IsAlive");
                IsAlive isAlive = new IsAlive();
                SendObject(isAlive);
                Debug.Log("Send IsAlive");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            yield return new WaitForSeconds(5f);
        }
    }
    
    private void LoadedLobbyPanel()
    {
        LobbyDataRequest lobbyDataRequest = new LobbyDataRequest {RequestingPlayerId = _clientId, RoomCode = _joinedRoomCode};
        SendObject(lobbyDataRequest);
    }

    public void ConnectToServer()
    {
        if (string.IsNullOrEmpty(_nameInput.text)) return;
        try
        {
            Debug.Log("Setting the client name.");
            _clientName = _nameInput.text;
            Debug.Log("Creating a new TCP Client");
            _client = new TcpClient();
            Debug.Log("Trying to connect to the server.");
            if (UseLocalhost)
            {
                _client.Connect("localhost", _port);
            }
            else
            {
                _client.Connect(ipAddress, _port);   
            }
            Debug.Log("Connected to server.");
            _connectedToServer = true;
            StartCoroutine(SendIsAlive());
        }
        catch (Exception e)
        {
            Debug.LogError("Could not connect to server:");
            Debug.LogError(e.Message);
        }
    }

    private void Update()
    {
        if (!_connectedToServer) return;
        
        try
        {
            if (_client.Available > 0)
            {
                Debug.Log("Bytes available! Reading...");
                byte[] inBytes = StreamUtil.Read(_client.GetStream());
                Packet inPacket = new Packet(inBytes);
                ISerializable inObject = inPacket.ReadObject();

                Debug.Log("Received: " + inObject);
                
                switch (inObject)
                {
                    case ClientDataRequest request:
                        HandleClientDataRequest(request);
                        break;
                    case PanelChange panelChange:
                        HandlePanelChange(panelChange);
                        break;
                    case LobbyCreateResponse response:
                        HandleLobbyCreateResponse(response);
                        break;
                    case LobbyDataResponse response:
                        HandleLobbyDataResponse(response);
                        break;
                    case LobbyJoinResponse response:
                        HandleLobbyJoinResponse(response);
                        break;
                    case LobbyLeaveResponse response:
                        HandleLobbyLeaveResponse(response);
                        break;
                    case MatchCreateResponse response:
                        HandleMatchCreateResponse(response);
                        break;
                    case SceneChange sceneChange:
                        HandleSceneChange(sceneChange);
                        break;
                    case MatchEndResponse response:
                        HandleMatchEndResponse(response);
                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            //_client.Close();
            //ConnectToServer();
        }
    }

    private void HandleMatchEndResponse(MatchEndResponse response)
    {
        NetworkClient.Disconnect();
    }
    
    private void HandleSceneChange(SceneChange sceneChange)
    {
        switch (sceneChange.SceneToSwitchTo)
        {
            case "GameScene":
                Debug.Log("Load Game Scene");
                SceneManager.LoadScene(_gameScene);
                break;
            case "MainMenu":
                Debug.Log("Load Main Menu Scene");
                SceneManager.LoadScene(_menuScene);
                break;
            case "LobbyMenu":
                Debug.Log("Load Lobby Menu Scene");
                SceneManager.LoadScene(_lobbyScene);
                break;
        }
    }
    
    private void HandleMatchCreateResponse(MatchCreateResponse response)
    {
        DataHandler.Port = (ushort)response.MatchPortNumber;
    }
    
    private void HandleLobbyLeaveResponse(LobbyLeaveResponse response)
    {
        _joinedRoomCode = "";
    }
    
    private void HandleLobbyJoinResponse(LobbyJoinResponse response)
    {
        _joinedRoomCode = response.RoomCode;

        if (response.ResponseCode == ResponseCode.Error)
        {
            Debug.LogWarning(response.ResponseMessage);
        }
        
        PlayerStateChangeRequest playerStateChangeRequest = new PlayerStateChangeRequest
            {PlayerId = _clientId, RequestedPlayerState = PlayerState.InLobby};
        
        SendObject(playerStateChangeRequest);
    }
    
    private void HandleLobbyDataResponse(LobbyDataResponse response)
    {
        // Update the lobby panel with the correct information
        EventBroker.CallUpdateLobbyDataEvent(response, _clientId);
    }
    
    private void HandleLobbyCreateResponse(LobbyCreateResponse response)
    {
        // TODO: better error handling
        _joinedRoomCode = response.RoomCode;

        PlayerStateChangeRequest playerStateChangeRequest = new PlayerStateChangeRequest
            {PlayerId = _clientId, RequestedPlayerState = PlayerState.InLobby};
        
        SendObject(playerStateChangeRequest);
    }
    
    private void HandlePanelChange(PanelChange panelChange)
    {
        // Change to a specific panel if the scene is MainMenu
        if (SceneManager.GetActiveScene().path != _menuScene) return;
        
        Debug.Log("Menu Scene is active");

        StartCoroutine(CallChangePanelEvent(panelChange.PanelToChangeTo));
    }

    IEnumerator CallChangePanelEvent(string panelToChangeTo)
    {
        yield return new WaitForSeconds(0.2f);
        EventBroker.CallChangePanelEvent(panelToChangeTo);
    }
    
    private void HandleClientDataRequest(ClientDataRequest request)
    {
        Client mainServerClient = request.Client;
        mainServerClient.Name = _clientName;
        mainServerClient.ClientType = ClientType.Client;

        _clientId = mainServerClient.Id;
        
        // Send

        ClientDataResponse response = new ClientDataResponse {Client = mainServerClient};
        SendObject(response);
    }

    private void SendObject(ISerializable pObject)
    {
        try
        {
            Debug.Log("Sending: " + pObject);
            Packet outPacket = new Packet();
            outPacket.Write(pObject);
            StreamUtil.Write(_client.GetStream(), outPacket.GetBytes());
        }
        catch (Exception e)
        {
            Debug.Log("Cannot send message!");
            Debug.Log(e.Message);
            
            _client.Close();
            ConnectToServer();
        }
    }

    public void CreateLobby()
    {
        if (string.IsNullOrEmpty(_descriptionInput.text)) return;

        LobbyCreateRequest lobbyCreateRequest = new LobbyCreateRequest
        {
            LobbyDescription = _descriptionInput.text, RequestingPlayerId = _clientId
        };
        
        SendObject(lobbyCreateRequest);
    }

    public void JoinLobby()
    {
        if (string.IsNullOrEmpty(_roomCodeInput.text)) return;

        LobbyJoinRequest lobbyJoinRequest = new LobbyJoinRequest
            {RequestingPlayerId = _clientId, RoomCode = _roomCodeInput.text};
        
        SendObject(lobbyJoinRequest);
    }

    public void ChangeReadyState()
    {
        ReadyStateChangeRequest readyStateChangeRequest = new ReadyStateChangeRequest {RequestingPlayerId = _clientId};
        SendObject(readyStateChangeRequest);
    }

    public void LeaveLobby()
    {
        LobbyLeaveRequest lobbyLeaveRequest = new LobbyLeaveRequest {RequestedPlayerId = _clientId};
        SendObject(lobbyLeaveRequest);
    }

    public void StartMatch()
    {
        MatchCreateRequest matchCreateRequest = new MatchCreateRequest
            {RequestingPlayerId = _clientId, RoomCode = _joinedRoomCode};
        
        SendObject(matchCreateRequest);
    }

    private void OnDestroy()
    {
        EventBroker.LoadedLobbyPanelEvent -= LoadedLobbyPanel;
        EventBroker.ConnectToServerEvent -= ConnectToServer;
        EventBroker.JoinLobbyEvent -= JoinLobby;
        EventBroker.HostLobbyEvent -= CreateLobby;
    }
}
