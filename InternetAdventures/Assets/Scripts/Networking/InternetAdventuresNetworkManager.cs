using System;
using System.Collections;
using System.Net.Sockets;
using kcp2k;
using Mirror;
using Shared;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Match;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class InternetAdventuresNetworkManager : NetworkManager
    {
        private KcpTransport kcpTransport;
        private bool IsServer = false;

        [SerializeField] private string _mainServerAddress = "localhost";
        [SerializeField] private int _mainServerPort = 55555;

        [Scene, SerializeField] private string _lobbyScene;

        private TcpClient _client;
        private string _serverRoomCode = "";
        private int _localPort = 0;
        private int _clientId = 0;
        private int _playersInEndZone = 0;
        private bool _endMatchPacketSend = false;

        public override void Awake()
        {
            base.Awake();

            string[] arguments = Environment.GetCommandLineArgs();
            
            kcpTransport = GetComponent<KcpTransport>();

            if (kcpTransport == null) return;

            // Server
            if (arguments.Length > 1 && arguments[1] == "-server")
            {
                Debug.Log(kcpTransport.Port);

                int.TryParse(arguments[2], out var port);
                
                Debug.Log("Starting server with port:" + port);

                _localPort = port;
                kcpTransport.Port = (ushort)port;

                _serverRoomCode = arguments[3];
            
                Debug.Log(kcpTransport.Port);

                IsServer = true;
            }
            else
            {
                Debug.Log("Giving client port:" + DataHandler.Port);
                _localPort = DataHandler.Port;
                kcpTransport.Port = DataHandler.Port;
            }
        }

        public override void Start()
        {
            base.Start();
            if (IsServer)
            {
                EventBroker.PlayerEnterMatchEndZoneEvent += PlayerEnterMatchEndZoneEvent;
                EventBroker.PlayerExitMatchEndZoneEvent += PlayerExitMatchEndZoneEvent;
                
                try
                {
                    _client = new TcpClient();
                    _client.Connect(_mainServerAddress, _mainServerPort);
                    Debug.Log("Connected new instance to main server!");
                }
                catch (Exception e)
                {
                    Debug.LogError("Could not connect to server:");
                    Debug.LogError(e.Message);
                }
            }
            else
            {
                Debug.Log("Starting Client!");
                EventBroker.LoadedLobbyPanelEvent += LoadedLobbyPanelEvent;
                StartClient();
            }
        }

        private void LoadedLobbyPanelEvent()
        {
            DestroyImmediate(this.gameObject);
        }

        private void PlayerEnterMatchEndZoneEvent()
        {
            _playersInEndZone++;
        }

        private void PlayerExitMatchEndZoneEvent()
        {
            _playersInEndZone--;
        }
        
        private void Update()
        {
            try
            {
                if (!IsServer) return;

                if (_playersInEndZone == 2 && !_endMatchPacketSend)
                {
                    MatchEndRequest matchEndRequest = new MatchEndRequest {RoomCode = _serverRoomCode, ServerId = _clientId};
                    SendObject(matchEndRequest);

                    _endMatchPacketSend = true;
                }
                
                if (_client.Available <= 0) return;
                
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
                    case StartServerInstance startServerInstance:
                        StartGameInstanceServer();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                _client.Close();
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            MatchEndRequest matchEndRequest = new MatchEndRequest {RoomCode = _serverRoomCode, ServerId = _clientId};
            SendObject(matchEndRequest);

            _endMatchPacketSend = true;
        }

        private void HandleClientDataRequest(ClientDataRequest request)
        {
            Client gameInstanceClient = request.Client;
            gameInstanceClient.Name = "GameInstance" + _localPort;
            gameInstanceClient.ClientType = ClientType.GameInstance;

            _clientId = gameInstanceClient.Id;

            ClientDataResponse response = new ClientDataResponse {Client = gameInstanceClient};
            SendObject(response);
        }
        
        public void StartGameInstanceServer()
        {
            if (!IsServer) return;
            
            Debug.Log("Starting server!");
            StartServer();
            StartCoroutine(SendStartSuccess());
        }

        IEnumerator SendStartSuccess()
        {
            yield return new WaitForSeconds(2f);
            
            Debug.Log("ServerStarted!");

            ServerStarted serverStarted = new ServerStarted
            {
                GameInstanceClientId = _clientId, GameInstancePort = _localPort, GameInstanceRoomCode = _serverRoomCode
            };
            
            SendObject(serverStarted);
        }

        private void SendObject(ISerializable pObject)
        {
            try
            {
                Debug.Log("Sending: " + pObject);
                Packet outPacket = new Packet();
                Debug.Log("Created packet " + pObject);

                if (pObject == null)
                {
                    Debug.Log("Object is null");
                }
                
                Debug.Log("Writing object data into packet");
                outPacket.Write(pObject);
                Debug.Log("Ended writing object data into packet");
                
                StreamUtil.Write(_client.GetStream(), outPacket.GetBytes());
            }
            catch (Exception e)
            {
                Debug.Log("Cannot send message!");
                Debug.Log(e.Message);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (IsServer)
            {
                EventBroker.PlayerEnterMatchEndZoneEvent -= PlayerEnterMatchEndZoneEvent;
                EventBroker.PlayerExitMatchEndZoneEvent -= PlayerExitMatchEndZoneEvent;
            }
            else
            {
                EventBroker.LoadedLobbyPanelEvent -= LoadedLobbyPanelEvent;
            }
        }
    }
}