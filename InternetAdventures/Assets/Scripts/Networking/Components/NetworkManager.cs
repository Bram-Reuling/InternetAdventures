using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Networking
{
    [RequireComponent(typeof(NetworkedPlayerManager))]
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private string server = "localhost";
        [SerializeField] private int port = 55555;
        
        private TcpClient client;

        public NetworkedPlayerManager networkedPlayerManager;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            ConnectToServer();
        }

        // Update is called once per frame
        void Update()
        {
            HandleIncomingPackets();
        }

        private void HandleIncomingPackets()
        {
            try
            {
                // Check if there is a message to read
                if (client.Available <= 0) return;
                
                // Read the bytes from the stream
                byte[] inBytes = StreamUtil.Read(client.GetStream());
                // Create a packet based on the bytes
                Packet inPacket = new Packet(inBytes);
                // Create an object of type ASerializable.
                ASerializable inObject = inPacket.ReadObject();

                switch (inObject)
                {
                    case ConnectionInfo info:
                        HandleConnectionInfo(info);
                        break;
                    case PlayerJoinEvent playerJoinEvent:
                        HandlePlayerJoinEvent(playerJoinEvent);
                        break;
                    case PlayerRemoveEvent playerRemoveEvent:
                        HandlePlayerRemoveEvent(playerRemoveEvent);
                        break;
                    case PlayerListResponse playerListResponse:
                        HandlePlayerListResponse(playerListResponse);
                        break;
                    case ClientHeartbeat heartBeatMessage:
                        HandleHeartbeat(heartBeatMessage);
                        break;
                    case PlayerMoveResponse pMoveResponse:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void HandleHeartbeat(ClientHeartbeat heartbeat)
        {
            Debug.Log("The number is: " + heartbeat.randomNumber);   
        }
        
        private void HandleConnectionInfo(ConnectionInfo info)
        {
            Debug.Log("ConnectionInfo Received");
            networkedPlayerManager.SetConnectionId(info.ID);
            
            try
            {
                Debug.Log("Sending PlayerListRequest");
                PlayerListRequest playerListRequest = new PlayerListRequest();
                Packet outPacket = new Packet();
                outPacket.Write(playerListRequest);
                StreamUtil.Write(client.GetStream(), outPacket.GetBytes());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void HandlePlayerJoinEvent(PlayerJoinEvent playerJoinEvent)
        {
            Debug.Log("PlayerJoinEvent Received");
            networkedPlayerManager.SpawnPlayer(playerJoinEvent.playerToAdd);
        }

        private void HandlePlayerRemoveEvent(PlayerRemoveEvent playerRemoveEvent)
        {
            Debug.Log("PlayerRemoveEvent Received");
            networkedPlayerManager.RemovePlayer(playerRemoveEvent.playerToRemove);
        }

        private void HandlePlayerListResponse(PlayerListResponse playerListResponse)
        {
            Debug.Log("PlayerListResponse Received");
            foreach (PlayerInfo playerInfo in playerListResponse.playerList)
            {
                networkedPlayerManager.SpawnPlayer(playerInfo);
            }
        }

        private void ConnectToServer()
        {
            try
            {
                // Create a new TCP Client
                client = new TcpClient();

                Debug.Log("Connecting to: " + server.ToString() +":" + port.ToString());
                // Connect to the TCP Listener at the specific server and port
                client.Connect(server, port);
                
                // Check if the client is connected to the server
                if (client.Connected)
                {
                    Debug.Log("Connected to: " + server.ToString() +":" + port.ToString());
                }
                
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
