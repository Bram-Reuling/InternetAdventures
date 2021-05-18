using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Shared;

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
                if (client.Available > 0)
                {
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                client.Close();
                ConnectToServer();
            }
        }
        
        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                client.Connect(server, port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
