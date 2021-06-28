using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using kcp2k;
using Mirror;
using Shared;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Lobby;
using Shared.protocol.Match;
using UnityEngine;

namespace Networking
{
    public class InternetAdventuresNetworkManager : NetworkManager
    {
        private KcpTransport kcpTransport;
        private bool IsServer = false;

        [SerializeField] private string _mainServerAddress = "localhost";
        [SerializeField] private int _mainServerPort = 55555;

        [Scene, SerializeField] private string _lobbyScene;

        private string filename = "";
        
        private TcpClient _client;
        private string _serverRoomCode = "";
        private int _localPort = 0;
        private int _clientId = 0;
        private int _playersInEndZone = 0;
        private bool _endMatchPacketSend = false;
        
        private bool SceneChanged = false;
        private string sceneChangedTo = "";
        private int sceneChangedForClients = 0;
        private bool loadedObjects = false;
        
        [SerializeField] private Material playerOneMaterialHead;
        [SerializeField] private Material playerOneMaterialFace;
        [SerializeField] private Material playerOneMaterialBody;
        
        [SerializeField] private Material playerTwoMaterialHead;
        [SerializeField] private Material playerTwoMaterialFace;
        [SerializeField] private Material playerTwoMaterialBody;

        public override void Awake()
        {
            base.Awake();

            string[] arguments = Environment.GetCommandLineArgs();
            
            kcpTransport = GetComponent<KcpTransport>();

            if (kcpTransport == null) return;

            // Server
            if (arguments.Length > 1 && arguments[1] == "-server")
            {
                Application.logMessageReceived += Log;
                
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
                EventBroker.SceneChangeEvent += ChangeSceneOnServer;
                EventBroker.MatchEndEvent += LoseWinEvent;
                
                NetworkServer.RegisterHandler<NameMessage>(ChangeUserName);
                
                try
                {
                    _client = new TcpClient();
                    _client.Connect(_mainServerAddress, _mainServerPort);
                    Debug.Log("Connected new instance to main server!");
                    StartCoroutine(SendIsAlive());
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
                
                NetworkClient.RegisterHandler<SkinMessage>(ChangeSkin);
                // Need to retrieve the player name and pass it to the server and sync it over the network
                StartClient();
                StartCoroutine(SendNetworkNameMessage());
            }
        }

        private void ChangeSkin(NetworkConnection connection, SkinMessage message)
        {
            Debug.Log("Received skin change");
            GameObject player = NetworkClient.localPlayer.gameObject;

            SkinnedMeshRenderer meshRenderer = player.transform.GetChild(1).GetChild(0).GetComponent<SkinnedMeshRenderer>();

            Material[] mats;
            
            switch (message.SkinIndex)
            {
                case 1:
                    mats = new Material[] {playerOneMaterialHead, playerOneMaterialFace, playerOneMaterialBody};
                    meshRenderer.materials = mats;
                    break;
                case 2:
                    mats = new Material[] {playerTwoMaterialHead, playerTwoMaterialFace, playerTwoMaterialBody};
                    meshRenderer.materials = mats;
                    break;
                default:
                    break;
            }
        }

        IEnumerator SendNetworkNameMessage()
        {
            yield return new WaitUntil(() => NetworkClient.isConnected);

            NameMessage nameMessage = new NameMessage
            {
                PlayerName = DataHandler.PlayerName
            };

            NetworkClient.Send(nameMessage);
        }

        private void ChangeUserName(NetworkConnection connection, NameMessage message)
        {
            Debug.Log("CHANGING NAME");
            foreach (var ownedObject in connection.clientOwnedObjects)
            {
                PlayerName playerName = ownedObject.GetComponent<PlayerName>();
                
                if (playerName == null) continue;
                
                playerName.ChangePlayerName(message.PlayerName);
            }
        }

        private void LoseWinEvent()
        {
            if (!_endMatchPacketSend)
            {
                Debug.Log("Lose win event triggered");
                EndMatch();
            }
        }

        private void ChangeSceneOnServer(string pScene)
        {
            ServerChangeScene(pScene);
            SceneChanged = true;
            sceneChangedTo = pScene;
        }
        
        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);

            if (SceneChanged)
            {
                sceneChangedForClients++;
            }
        }
        
        IEnumerator SendIsAlive()
        {
            while (_client.Connected)
            {
                IsAlive isAlive = new IsAlive();
                SendObject(isAlive);
                yield return new WaitForSeconds(5f);
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
        
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            // instantiating a "Player" prefab gives it the name "Player(clone)"
            // => appending the connectionId is WAY more useful for debugging!
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

            // Trigger event on the client based on how many players have been added

            SkinnedMeshRenderer meshRenderer = player.transform.GetChild(1).GetChild(0).GetComponent<SkinnedMeshRenderer>();
            
            if (DataHandler.NoOfPlayers == 1)
            {
                Material[] mats = new Material[] {playerOneMaterialHead, playerOneMaterialFace, playerOneMaterialBody};
                meshRenderer.materials = mats;
                DataHandler.NoOfPlayers = 2;
            }
            else
            {
                Material[] mats = new Material[] {playerTwoMaterialHead, playerTwoMaterialFace, playerTwoMaterialBody};
                meshRenderer.materials = mats;
            }

            if (NetworkServer.AddPlayerForConnection(conn, player))
            {
                Debug.Log("Sending Skin change to player");
                
                SkinMessage skinMessage = new SkinMessage
                {
                    SkinIndex = DataHandler.NoOfPlayers
                };
                
                conn.Send(skinMessage);
            }
        }
        
        private void Update()
        {
            if (!loadedObjects)
            {
                NetworkServer.SpawnObjects();
                loadedObjects = true;
            }
            
            try
            {
                if (!IsServer) return;

                if (_playersInEndZone == 2 && !_endMatchPacketSend)
                {
                    Debug.Log("Every player is in the endzone");
                    EndMatch();
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
                    case LobbyDataResponse response:
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                _client.Close();
            }
        }

        private void EndMatch()
        {
            MatchEndRequest matchEndRequest = new MatchEndRequest {RoomCode = _serverRoomCode, ServerId = _clientId};
            SendObject(matchEndRequest);

            _endMatchPacketSend = true;
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log("Player disconnected!");
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
                EventBroker.SceneChangeEvent += ChangeSceneOnServer;
                EventBroker.MatchEndEvent += LoseWinEvent;
                Application.logMessageReceived -= Log;
            }
            else
            {
                EventBroker.LoadedLobbyPanelEvent -= LoadedLobbyPanelEvent;
            }
        }
        
        public void Log(string logString, string stackTrace, LogType type)
        {
            if (filename == "")
            {
                string d = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
                System.IO.Directory.CreateDirectory(d);
                filename = d + $"/server-log.txt";
            }
 
            try {
                System.IO.File.AppendAllText(filename, logString + "\n");
            }
            catch { }
        }
    }
}