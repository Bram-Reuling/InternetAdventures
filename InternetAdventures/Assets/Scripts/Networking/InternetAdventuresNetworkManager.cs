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

namespace Networking
{
    public class InternetAdventuresNetworkManager : NetworkManager
    {
        private KcpTransport kcpTransport;
        private bool IsServer = false;

        [SerializeField] private string _mainServerAddress = "localhost";
        [SerializeField] private int _mainServerPort = 55555;

        private TcpClient _client;
        private string _serverRoomCode;
        private int _localPort;
        private int _clientId;

        public override void Awake()
        {
            base.Awake();

            string[] arguments = Environment.GetCommandLineArgs();
            
            kcpTransport = GetComponent<KcpTransport>();

            if (kcpTransport == null) return;

            // Server
            if (arguments[1] == "-server")
            {
                Debug.Log(kcpTransport.Port);

                ushort port;
                ushort.TryParse(arguments[2], out port);

                _localPort = port;
                kcpTransport.Port = port;
            
                Debug.Log(kcpTransport.Port);

                IsServer = true;
            }
        }

        public override void Start()
        {
            base.Start();
            if (IsServer)
            {
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
        }

        private void Update()
        {
            try
            {
                if (_client.Available > 0 && IsServer)
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
                        case StartServerInstance startServerInstance:
                            StartGameInstanceServer();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                _client.Close();
            }
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
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
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
            
            yield return null;
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
            }
        }
    }
}