using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
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

                SVector3 spawnPosition = new SVector3(0, 2, 0);
                
                PlayerInfo playerInfo = new PlayerInfo {ID = GenerateRandomID() ,position = spawnPosition};

                connectedPlayers.Add(channel, playerInfo);

                Log.LogInfo("Accepted new client with ID: " + playerInfo.ID, this, ConsoleColor.White);
                
                foreach (KeyValuePair<TcpClient, PlayerInfo> pair in connectedPlayers)
                {
                    SendPlayerListUpdateEvent(pair.Key, PlayerListUpdateType.PlayerAdded);
                }
            }
        }

        private int GenerateRandomID()
        {
            List<int> allPlayerIds = connectedPlayers.Select(player => player.Value.ID).ToList();

            int randomId = new Random().Next(0, 100000);

            while (allPlayerIds.Contains(randomId))
            {
                randomId = new Random().Next(0, 100000);
            }

            return randomId;
        }
        
        private void ProcessExistingClients()
        {
            foreach (KeyValuePair<TcpClient,PlayerInfo> connectedPlayer in connectedPlayers)
            {
                if (connectedPlayer.Key.Available == 0) continue;

                byte[] inBytes = StreamUtil.Read(connectedPlayer.Key.GetStream());
                Packet inPacket = new Packet(inBytes);
                ASerializable inObject = inPacket.ReadObject();
                
                Log.LogInfo("Received packet:" + inObject, this, ConsoleColor.DarkBlue);

                if (inObject is PlayerMoveRequest playerMoveRequest)
                {
                    HandlePlayerMoveRequest(playerMoveRequest, connectedPlayer.Value);
                }
            }
        }

        private void HandlePlayerMoveRequest(PlayerMoveRequest pMoveRequest, PlayerInfo requestingPlayer)
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

        private void SendPlayerListUpdateEvent(TcpClient client, PlayerListUpdateType pType)
        {
            PlayerListUpdateEvent playerListUpdateEvent = new PlayerListUpdateEvent();

            playerListUpdateEvent.updateType = pType;
            
            List<PlayerInfo> allPlayers = new List<PlayerInfo>();

             foreach (KeyValuePair<TcpClient,PlayerInfo> players in connectedPlayers)
             {
                 allPlayers.Add(players.Value);
             }

            playerListUpdateEvent.updatedPlayerList = allPlayers;

            Log.LogInfo("Sending PlayerListUpdateEvent!", this, ConsoleColor.White);
            SendObject(client, playerListUpdateEvent);
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
                SendPlayerListUpdateEvent(player.Key, PlayerListUpdateType.PlayerRemoved);
            }
        }
    }
}