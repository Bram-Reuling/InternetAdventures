using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Networking.Server
{
    public class GameServer : MonoBehaviour
    {
        private TcpListener listener;
        
        private void Awake()
        {
            try
            {
                Console.WriteLine("Starting server on port 55555");

                listener = new TcpListener(IPAddress.Any, 55555);
                listener.Start();
            
                Console.WriteLine("Started server!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }
    }
}