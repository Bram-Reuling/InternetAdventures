using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Networking
{
    public class GameServer : MonoBehaviour
    {
        private TcpListener listener;
        
        private void Awake()
        {
            try
            {
                Log.LogInfo("Starting server on port 55555!", this, ConsoleColor.DarkCyan);
                
                listener = new TcpListener(IPAddress.Any, 55555);
                listener.Start();
            
                Log.LogInfo("Started server!", this, ConsoleColor.Green);
            }
            catch (Exception e)
            {
                Log.LogInfo("Failed to start server:", this, ConsoleColor.Red);
                Log.LogInfo(e, this, ConsoleColor.Red);
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