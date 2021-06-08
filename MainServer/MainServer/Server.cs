using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Shared;
using Shared.model;
using Shared.protocol;

namespace MainServer
{
    class Server
    {
        private TcpListener _listener;
        private Dictionary<Player, TcpClient> _connectedPlayers = new Dictionary<Player, TcpClient>();
        private List<Room> _rooms = new List<Room>();

        public static void Main(string[] args)
        {
            Server server = new Server();
            server.Run();
        }

        private void Run()
        {
            try
            {
                Console.WriteLine("Server started on port 55555");

                _listener = new TcpListener(IPAddress.Any, 55555);
                _listener.Start();

                while (true)
                {
                    ProcessNewClients();
                    ProcessExistingClients();
                    ProcessFaultyClients();
                
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ProcessNewClients()
        {
            try
            {
                while (_listener.Pending())
                {
                    Player newPlayer = new Player();
                    GeneratePlayerId(ref newPlayer);

                    TcpClient client = _listener.AcceptTcpClient();
                    
                    _connectedPlayers.Add(newPlayer, client);

                    PlayerDataRequest playerDataRequest = new PlayerDataRequest();
                    playerDataRequest.Player = newPlayer;
                    
                    SendObject(new KeyValuePair<Player, TcpClient>(newPlayer, client), playerDataRequest);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ProcessExistingClients()
        {
            foreach (KeyValuePair<Player,TcpClient> player in _connectedPlayers)
            {
                if (player.Value.Available == 0) continue;

                byte[] inBytes = StreamUtil.Read(player.Value.GetStream());
                Packet inPacket = new Packet(inBytes);
                ISerializable inObject = inPacket.ReadObject();

                Console.WriteLine("Received: " + inObject);

                switch (inObject)
                {
                    case PlayerDataResponse response:
                        HandlePlayerDataResponse(response);
                        break;
                    case PlayerStateChangeRequest request:
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandlePlayerDataResponse(PlayerDataResponse response)
        {
            Player player = _connectedPlayers.FirstOrDefault(p => p.Key.Id == response.Player.Id).Key;
            player.Name = response.Player.Name;
        }

        private void HandlePlayerStateChangeRequest(PlayerStateChangeRequest request)
        {
            // Do some authoritative shit        
            
            // Send some shit to the client
        }
        
        private void ProcessFaultyClients()
        {
        }

        private void GeneratePlayerId(ref Player playerObject)
        {
            var rnd = new Random();
            int id = 0;
            bool result = true;

            while (result)
            {
                id = rnd.Next(10001);
                result = _connectedPlayers.Any(p => p.Key.Id == id);
            }

            playerObject.Id = id;
        }

        private void RemovePlayer(KeyValuePair<Player, TcpClient> player)
        {
            try
            {
                player.Value.Close();
                _connectedPlayers.Remove(player.Key);

                switch (player.Key.PlayerState)
                {
                    case PlayerState.SearchingForLobby:
                        // Do not notify every client
                        break;
                    case PlayerState.InLobby:
                        // Notify every client in the lobby
                        break;
                    case PlayerState.InGame:
                        // Notify every client in the game
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SendObject(KeyValuePair<Player, TcpClient> player, ISerializable outObject)
        {
            try
            {
                Packet outPacket = new Packet();
                outPacket.Write(outObject);
                
                StreamUtil.Write(player.Value.GetStream(), outPacket.GetBytes());
            }
            catch (Exception e)
            {
                // Remove the faulty client
                Console.WriteLine(e);
                if (!player.Value.Connected)
                {
                    RemovePlayer(player);
                }
            }
        }
    }
}