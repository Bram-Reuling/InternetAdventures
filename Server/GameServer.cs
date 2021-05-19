using System;
using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<TcpClient, PlayerInfo> connectedPlayers = new Dictionary<TcpClient, PlayerInfo>();
        
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
                
                TcpClient channel = listener.AcceptTcpClient();

                PlayerInfo playerInfo = new PlayerInfo();

                playerInfo.position = new SVector3();
                
                connectedPlayers.Add(channel, playerInfo);

                foreach (KeyValuePair<TcpClient, PlayerInfo> pair in connectedPlayers)
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
            List<TcpClient> clients = connectedPlayers.Keys.ToList();

            foreach (TcpClient client in clients)
            {
                Ping ping = new Ping();
                SendObject(client, ping);
            }
        }

        private void SendPlayerJoinedEvent(TcpClient client)
        {
            PlayerJoinedEvent playerJoinedEvent = new PlayerJoinedEvent();

            List<PlayerInfo> playerData = new List<PlayerInfo>();

             foreach (KeyValuePair<TcpClient,PlayerInfo> players in connectedPlayers)
             {
                 playerData.Add(players.Value);
             }

            playerJoinedEvent.players = playerData;

            //playerJoinedEvent.justANumber = 500;
            
            Log.LogInfo("Sending PlayerJoinedEvent!", this, ConsoleColor.White);
            SendObject(client, playerJoinedEvent);
        }

        private void SendObject(TcpClient client, ASerializable pObject)
        {
            try
            {
                Packet outPacket = new Packet();
                outPacket.Write(pObject);
                StreamUtil.Write(client.GetStream(), outPacket.GetBytes());
            }
            catch (Exception e)
            {
                Log.LogInfo("Could not send object to client! Client is perhaps faulthy?", this, ConsoleColor.Red);
                RemoveClient(client);
            }
        }

        private void RemoveClient(TcpClient client)
        {
            client.Close();
            connectedPlayers.Remove(client);

            foreach (KeyValuePair<TcpClient,PlayerInfo> player in connectedPlayers)
            {
                // Send Player Removed Event
            }
        }
    }
}