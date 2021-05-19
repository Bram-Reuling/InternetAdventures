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
                
                
                if (inObject is PlayerJoinedEvent pEvent)
                {
                    Debug.Log("PlayerJoinedEvent Received");

                    //TODO: Spawn the player + all the other players
                }
                else if (inObject is Ping) {}
                else if (inObject is PlayerMoveResponse pMoveResponse)
                {
                    
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
                
                client = new TcpClient();

                Debug.Log("Connecting to: " + server.ToString() +":" + port.ToString());
                client.Connect(server, port);
                
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
