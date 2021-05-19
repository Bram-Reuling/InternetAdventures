using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Shared;
using Ping = Shared.Ping;

namespace Networking
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private string server = "localhost";
        [SerializeField] private int port = 55555;
        
        private TcpClient client;

        private List<PlayerInfo> players;
        
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

                Debug.Log("Message is available");

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

        private static void ProcessPlayerListUpdateEvents(PlayerListUpdateEvent pEvent)
        {
            Debug.Log("PlayerListUpdateEvent Received");

            PlayerListUpdateType typeOfUpdate = pEvent.updateType;

            switch (typeOfUpdate)
            {
                case PlayerListUpdateType.PlayerRemoved:
                    // Destroy the removed player.
                    break;
                case PlayerListUpdateType.PlayerAdded:
                    // Spawn the added player.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
