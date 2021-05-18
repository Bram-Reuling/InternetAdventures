using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Shared;

namespace Server
{
    class GameServer
    {
        public static void Main(string[] args)
        {
            GameServer server = new GameServer();
            server.Run();
        }

        private TcpListener listener;

        private Dictionary<TcpMessageChannel, PlayerInfo> connectedPlayers = new Dictionary<TcpMessageChannel, PlayerInfo>();
        
        private void Run()
        {
            Log.LogInfo("Server started on port 55555", this, ConsoleColor.White);

            listener = new TcpListener(IPAddress.Any, 55555);
            listener.Start();

            while (true)
            {
                ProcessNewClients();
                ProcessExistingClients();
                ProcessFaulthyClients();
                
                Thread.Sleep(100);
            }
        }

        private void ProcessNewClients()
        {
            if (listener.Pending())
            {
                Log.LogInfo("Accepting new client...", this, ConsoleColor.White);
                
                TcpMessageChannel channel = new TcpMessageChannel(listener.AcceptTcpClient());
                
                connectedPlayers.Add(channel, new PlayerInfo());

                foreach (KeyValuePair<TcpMessageChannel, PlayerInfo> pair in connectedPlayers)
                {
                    SendPlayerJoinedEvent(pair.Key);
                }
            }
        }
        
        private void ProcessExistingClients()
        {
            
        }
        
        private void ProcessFaulthyClients()
        {
            
        }

        private void SendPlayerJoinedEvent(TcpMessageChannel client)
        {
            PlayerJoinedEvent playerJoinedEvent = new PlayerJoinedEvent();

            List<PlayerInfo> playerData = new List<PlayerInfo>();

            foreach (KeyValuePair<TcpMessageChannel,PlayerInfo> players in connectedPlayers)
            {
                playerData.Add(players.Value);
            }

            playerJoinedEvent.players = playerData;
            
            SendObject(client, playerJoinedEvent);
        }

        private void SendObject(TcpMessageChannel client, ASerializable pObject)
        {
            client.SendMessage(pObject);
        }
    }
}