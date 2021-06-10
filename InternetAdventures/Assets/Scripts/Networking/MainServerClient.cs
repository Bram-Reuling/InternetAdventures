using System;
using System.Collections;
using System.Collections.Generic;
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
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainServerClient : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private TMP_InputField _roomCodeInput;
    [SerializeField] private string _server = "localhost";
    [SerializeField] private int _port = 55555;

    [Scene, SerializeField] private string _menuScene;
    [Scene, SerializeField] private string _gameScene;
    
    private TcpClient _client;
    private bool _connectedToServer = false;

    [SerializeField] private string _clientName = "Bram";
    private int _clientId = 0;

    private string _joinedRoomCode = "";

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        //ConnectToServer();
        EventBroker.LoadedLobbyPanelEvent += LoadedLobbyPanel;
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
            _clientName = _nameInput.text;
            
            _client = new TcpClient();
            _client.Connect(_server, _port);
            _connectedToServer = true;
            Debug.Log("Connected to server.");
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
                    default:
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            _client.Close();
            ConnectToServer();
        }
    }

    private void HandleSceneChange(SceneChange sceneChange)
    {
        switch (sceneChange.SceneToSwitchTo)
        {
            case "GameScene":
                Debug.Log("Load Game Scene");
                SceneManager.LoadScene(_gameScene);
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

        EventBroker.CallChangePanelEvent(panelChange.PanelToChangeTo);
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
}
