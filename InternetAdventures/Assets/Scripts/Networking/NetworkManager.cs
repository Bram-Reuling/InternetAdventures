using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using Shared;
using UnityEngine.InputSystem;
using Ping = Shared.Ping;

namespace Networking
{
    [RequireComponent(typeof(NetworkedPlayerManager))]
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private string server = "localhost";
        [SerializeField] private int port = 55555;
        
        private TcpClient client;

        private List<PlayerInfo> players;

        public NetworkedPlayerManager networkedPlayerManager;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            players = new List<PlayerInfo>();

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

                // Process PlayerListUpdateEvents
                if (inObject is PlayerListUpdateEvent pEvent)
                {
                    ProcessPlayerListUpdateEvents(pEvent);
                }
                // NOTE: LEAVE THIS EMPTY & DONT DELETE THIS.
                // This is for the ping system from the server to check if the client is still connected.
                else if (inObject is Ping)
                {
                }
                else if (inObject is PlayerMoveResponse pMoveResponse)
                {
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void ProcessPlayerListUpdateEvents(PlayerListUpdateEvent pEvent)
        {
            try
            {
                Debug.Log("PlayerListUpdateEvent Received");

                PlayerListUpdateType typeOfUpdate = pEvent.updateType;
            
                Debug.Log("Update Type: " + typeOfUpdate);

                List<PlayerInfo> newPlayerList = pEvent.updatedPlayerList;

                Debug.Log("Length of the list: " + newPlayerList.Count);
            
                switch (typeOfUpdate)
                {
                    case PlayerListUpdateType.PlayerRemoved:
                        // Destroy the removed player.
                        Debug.Log("Player Removed Case");
                        IEnumerable<PlayerInfo> differenceListRemove = players.Except(newPlayerList);
                        foreach (PlayerInfo player in differenceListRemove)
                        {
                            networkedPlayerManager.RemovePlayer(player);
                        }

                        players = newPlayerList;
                        break;
                    case PlayerListUpdateType.PlayerAdded:
                        Debug.Log("Player Added Case");
                        // Spawn the added player.
                        IEnumerable<PlayerInfo> differenceListAdd = newPlayerList.Except(players);

                        foreach (PlayerInfo player in differenceListAdd)
                        {
                            Debug.Log("Player thingy ID: " + player.ID);
                            networkedPlayerManager.SpawnPlayer(player);
                        }

                        players = newPlayerList;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
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
