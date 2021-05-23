using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Random = System.Random;

namespace Networking
{
    public class GameServer : MonoBehaviour
    {
        private TcpListener listener;
        private Dictionary<TcpClient, PlayerInfo> connectedPlayers = new Dictionary<TcpClient, PlayerInfo>();
        
        private void Awake()
        {
            try
            {
                Log.LogInfo("Starting server on port 55555!", this, ConsoleColor.DarkCyan);

                Console.WriteLine("Starting server on port 55555!");
                
                listener = new TcpListener(IPAddress.Any, 55555);
                listener.Start();
            
                Log.LogInfo("Started server!", this, ConsoleColor.Green);
                Console.WriteLine("Started server!");
            }
            catch (Exception e)
            {
                Log.LogInfo("Failed to start server:", this, ConsoleColor.Red);
                Console.WriteLine("Failed to start server:");
                Log.LogInfo(e, this, ConsoleColor.Red);
                Console.WriteLine(e);
            }
        }

        private void Update()
        {
            ProcessNewClients();
            ProcessExistingClients();
            ProcessFaulthyClients();
                
            Thread.Sleep(100);
        }

        /// <summary>
        /// Checks if there are new clients trying to connect.
        /// </summary>
        private void ProcessNewClients()
        {
            // If there are no clients pending, quit the functions
            if (!listener.Pending()) return;
            
            Log.LogInfo("Accepting new client...", this, ConsoleColor.White);
            Console.WriteLine("Accepting new client...");
                
            TcpClient channel = listener.AcceptTcpClient();

            Vector3 spawnPosition = new Vector3(0, 2, 0);
                
            PlayerInfo playerInfo = new PlayerInfo {ID = GenerateRandomID() ,position = spawnPosition};

            connectedPlayers.Add(channel, playerInfo);

            Log.LogInfo("Accepted new client with ID: " + playerInfo.ID, this, ConsoleColor.White);
            Console.WriteLine("Accepted new client with ID: " + playerInfo.ID);
                
            SendConnectionInfo(channel, playerInfo);
                
            foreach (var pair in connectedPlayers.Where(pair => pair.Value.ID != playerInfo.ID))
            {
                SendPlayerJoinEvent(pair.Key, playerInfo);
            }
            
        }

        /// <summary>
        /// Generates a random ID.
        /// </summary>
        /// <returns>Int: ID</returns>
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

        /// <summary>
        /// Checks if there are incoming packets and processes them.
        /// </summary>
        private void ProcessExistingClients()
        {
            foreach (KeyValuePair<TcpClient,PlayerInfo> connectedPlayer in connectedPlayers)
            {
                if (connectedPlayer.Key.Available == 0) continue;

                byte[] inBytes = StreamUtil.Read(connectedPlayer.Key.GetStream());
                Packet inPacket = new Packet(inBytes);
                ASerializable inObject = inPacket.ReadObject();
                
                Log.LogInfo("Received packet:" + inObject, this, ConsoleColor.DarkBlue);
                Console.WriteLine("Received packet:" + inObject);

                switch (inObject)
                {
                    case PlayerMoveRequest playerMoveRequest:
                        HandlePlayerMoveRequest(playerMoveRequest, connectedPlayer);
                        break;
                    case PlayerListRequest playerListRequest:
                        HandlePlayerListRequest(connectedPlayer);
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandlePlayerListRequest(KeyValuePair<TcpClient,PlayerInfo> requestingPlayer)
        {
            Log.LogInfo("Sending PlayerListResponse!", this, ConsoleColor.Cyan);
            Console.WriteLine("Sending PlayerListResponse!");
            PlayerListResponse playerListResponse = new PlayerListResponse
            {
                playerList = connectedPlayers.Select(player => player.Value).ToList()
            };


            SendObject(requestingPlayer.Key, playerListResponse);
        }

        private void HandlePlayerMoveRequest(PlayerMoveRequest pMoveRequest, KeyValuePair<TcpClient,PlayerInfo> requestingPlayer)
        {
        }

        private void ProcessFaulthyClients()
        {
            List<TcpClient> clients = connectedPlayers.Keys.ToList();

            foreach (TcpClient client in clients)
            {
                ClientHeartbeat heartbeat = new ClientHeartbeat { randomNumber = 5122345 };
                SendObject(client, heartbeat);
            }
        }
        
        private void SendConnectionInfo(TcpClient receiver, PlayerInfo info)
        {
            ConnectionInfo connectionInfo = new ConnectionInfo {ID = info.ID};
            
            Log.LogInfo("Sending ConnectionInfo!", this, ConsoleColor.Cyan);
            Console.WriteLine("Sending ConnectionInfo!");
            
            SendObject(receiver, connectionInfo);
        }
        
        private void SendPlayerJoinEvent(TcpClient receiver, PlayerInfo newPlayer)
        {
            PlayerJoinEvent playerJoinEvent = new PlayerJoinEvent {playerToAdd = newPlayer};
            
            Log.LogInfo("Sending PlayerJoinEvent!", this, ConsoleColor.Cyan);
            Console.WriteLine("Sending PlayerJoinEvent!");
            SendObject(receiver, playerJoinEvent);
        }
        
        private void SendPlayerRemoveEvent(TcpClient receiver, PlayerInfo removePlayer)
        {
            PlayerRemoveEvent playerRemoveEvent = new PlayerRemoveEvent {playerToRemove = removePlayer};
            
            Log.LogInfo("Sending PlayerJoinEvent!", this, ConsoleColor.Cyan);
            Console.WriteLine("Sending PlayerJoinEvent!");
            SendObject(receiver, playerRemoveEvent);
        }
        
        private void SendObject(TcpClient receiver, ASerializable pObject)
        {
            try
            {
                Packet outPacket = new Packet();
                outPacket.Write(pObject);
                StreamUtil.Write(receiver.GetStream(), outPacket.GetBytes());
            }
            catch (Exception e)
            {
                Log.LogInfo("Could not send object to client! Client is perhaps faulthy?", this, ConsoleColor.Red);
                Console.WriteLine("Could not send object to client! Client is perhaps faulthy?");
                RemoveClient(receiver);
            }
        }
        
        private void RemoveClient(TcpClient client)
        {
            client.Close();
            PlayerInfo playerToRemove = connectedPlayers[client];
            connectedPlayers.Remove(client);

            foreach (KeyValuePair<TcpClient,PlayerInfo> player in connectedPlayers)
            {
                SendPlayerRemoveEvent(player.Key, playerToRemove);
            }
        }
    }
}