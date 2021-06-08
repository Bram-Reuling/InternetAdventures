using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Shared;
using Shared.log;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Lobby;

namespace MainServer
{
    class Server
    {
        private TcpListener _listener;
        private Dictionary<Client, TcpClient> _connectedPlayers = new Dictionary<Client, TcpClient>();
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
                Log.LogInfo("Server starting on port 55555", this, ConsoleColor.White);
                //Console.WriteLine("Server started on port 55555");

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
                    Log.LogInfo("New client pending!", this, ConsoleColor.Green);
                    Client newClient = new Client();
                    GeneratePlayerId(ref newClient);

                    Log.LogInfo("Accepted new client.", this, ConsoleColor.Green);
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    
                    _connectedPlayers.Add(newClient, tcpClient);

                    ClientDataRequest playerDataRequest = new ClientDataRequest();
                    playerDataRequest.Client = newClient;
                    
                    SendObject(new KeyValuePair<Client, TcpClient>(newClient, tcpClient), playerDataRequest);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ProcessExistingClients()
        {
            foreach (KeyValuePair<Client,TcpClient> player in _connectedPlayers)
            {
                if (player.Value.Available == 0) continue;

                byte[] inBytes = StreamUtil.Read(player.Value.GetStream());
                Packet inPacket = new Packet(inBytes);
                ISerializable inObject = inPacket.ReadObject();

                Log.LogInfo($"Received: {inObject}", this, ConsoleColor.Blue);

                switch (inObject)
                {
                    case ClientDataResponse response:
                        HandleClientDataResponse(response);
                        break;
                    case PlayerStateChangeRequest request:
                        HandlePlayerStateChangeRequest(request);
                        break;
                    case LobbyCreateRequest request:
                        HandleLobbyCreateRequest(request);
                        break;
                    case LobbyDataRequest request:
                        HandleLobbyDataRequest(request);
                        break;
                    case LobbyJoinRequest request:
                        HandleLobbyJoinRequest(request);
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleLobbyJoinRequest(LobbyJoinRequest request)
        {
            // Check if room with room code exists
            Room room = _rooms.Find(r => r.RoomCode == request.RoomCode);

            KeyValuePair<Client, TcpClient> clientPair =
                _connectedPlayers.FirstOrDefault(c => c.Key.Id == request.RequestingPlayerId);
            
            if (room == null)
            {
                Log.LogInfo($"Lobby not found with code: {request.RoomCode}!", this, ConsoleColor.Red);
                LobbyJoinResponse lobbyJoinResponse = new LobbyJoinResponse
                    {ResponseCode = ResponseCode.Error, ResponseMessage = "No room found with room code!"};
                
                SendObject(clientPair, lobbyJoinResponse);
            }
            else
            {
                Log.LogInfo("Lobby found!", this, ConsoleColor.DarkGreen);
                LobbyJoinResponse lobbyJoinResponse = new LobbyJoinResponse
                    {ResponseCode = ResponseCode.Ok, RoomCode = request.RoomCode};
                
                _rooms.Find(r => r.RoomCode == request.RoomCode)?.Players.Add(clientPair.Key);

                SendObject(clientPair, lobbyJoinResponse);

                LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                    {Lobby = _rooms.Find(r => r.RoomCode == request.RoomCode), ResponseCode = ResponseCode.Ok};

                KeyValuePair<Client, TcpClient> otherClientPair =
                    _connectedPlayers.FirstOrDefault(c => c.Key.Id == room.Players[0].Id);
                
                SendObject(otherClientPair, lobbyDataResponse);

                // Tell the client to switch to the lobby panel
                PanelChange panelChange = new PanelChange {PanelToChangeTo = "LobbyPanel"};
                SendObject(clientPair, panelChange);
            }
        }
        
        private void HandleLobbyDataRequest(LobbyDataRequest request)
        {
            Room room = _rooms.Find(p => p.RoomCode == request.RoomCode);

            if (room == null) return;
            
            LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                {Lobby = room, ResponseCode = ResponseCode.Ok};
                
            KeyValuePair<Client, TcpClient> clientPair =
                _connectedPlayers.FirstOrDefault(c => c.Key.Id == request.RequestingPlayerId);
                
            SendObject(clientPair, lobbyDataResponse);
        }
        
        private void HandleLobbyCreateRequest(LobbyCreateRequest request)
        {
            // Generate room code
            string roomCode = GenerateRoomCode();
            Log.LogInfo($"Generated RoomCode: {roomCode}", this, ConsoleColor.Magenta);
            // Create room
            Room room = new Room { Id = _rooms.Count + 1, RoomCode = roomCode };
            Log.LogInfo("Created a new room!", this, ConsoleColor.Green);
            // Add user to room
            KeyValuePair<Client, TcpClient> clientPair =
                _connectedPlayers.FirstOrDefault(c => c.Key.Id == request.RequestingPlayerId);
            room.Players.Add(clientPair.Key);
            Log.LogInfo("Added the requesting player to the players list", this, ConsoleColor.Green);
            // Add room to the room list
            _rooms.Add(room);
            Log.LogInfo("Added the room to the rooms list with ID: " + room.Id, this, ConsoleColor.Green);
            // Send LobbyCreateResponse
            LobbyCreateResponse lobbyCreateResponse = new LobbyCreateResponse
                {ResponseCode = ResponseCode.Ok, RoomCode = roomCode};
            
            SendObject(clientPair, lobbyCreateResponse);
            
            // Tell the client to switch to the lobby panel
            PanelChange panelChange = new PanelChange {PanelToChangeTo = "LobbyPanel"};
            SendObject(clientPair, panelChange);

        }

        private string GenerateRoomCode()
        {
            Random rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code = "";
            bool result = true;

            while (result)
            {
                code = new string(Enumerable.Repeat(chars, 5).Select(s => s[rnd.Next(s.Length)]).ToArray());
                result = _rooms.Any(c => c.RoomCode == code);
            }

            return code;
        }
        
        private void HandleClientDataResponse(ClientDataResponse response)
        {
            KeyValuePair<Client, TcpClient> clientPair =
                _connectedPlayers.FirstOrDefault(p => p.Key.Id == response.Client.Id);
            Client client = clientPair.Key;
            Log.LogInfo($"Client Name: {response.Client.Name}", this, ConsoleColor.Blue);
            client.Name = response.Client.Name;
            Log.LogInfo($"Client Type: {response.Client.ClientType}", this, ConsoleColor.Blue);
            client.ClientType = response.Client.ClientType;
            
            // Since this method is only called when a client is connected
            // Send a PanelChange packet
            
            PanelChange panelChange = new PanelChange { PanelToChangeTo = "MainPanel" };
            Log.LogInfo($"Sending: {panelChange}", this, ConsoleColor.DarkBlue);
            SendObject(clientPair, panelChange);
        }

        private void HandlePlayerStateChangeRequest(PlayerStateChangeRequest request)
        {
            // Secure this
            KeyValuePair<Client, TcpClient> clientPair =
                _connectedPlayers.FirstOrDefault(c => c.Key.Id == request.PlayerId);
            
            PlayerStateChangeResponse playerStateChangeResponse = new PlayerStateChangeResponse
                {NewPlayerState = request.RequestedPlayerState, PlayerId = request.PlayerId};
            
            SendObject(clientPair, playerStateChangeResponse);
        }
        
        private void ProcessFaultyClients()
        {
        }

        private void GeneratePlayerId(ref Client playerObject)
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

        private void RemovePlayer(KeyValuePair<Client, TcpClient> player)
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

        private void SendObject(KeyValuePair<Client, TcpClient> player, ISerializable outObject)
        {
            try
            {
                Log.LogInfo($"Sending {outObject} to player with ID: {player.Key.Id}", this, ConsoleColor.Cyan);
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