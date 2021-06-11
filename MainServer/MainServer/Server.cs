using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Shared;
using Shared.log;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Lobby;
using Shared.protocol.Match;
using Shared.protocol.protocol;

namespace MainServer
{
    class Server
    {
        private TcpListener _listener;
        private Dictionary<Client, TcpClient> _connectedPlayers = new Dictionary<Client, TcpClient>();
        private List<Room> _rooms = new List<Room>();
        private List<int> _ports = new List<int> {55556, 55557, 55558, 55559};

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
                    case ReadyStateChangeRequest request:
                        HandleReadyStateChangeRequest(request);
                        break;
                    case LobbyLeaveRequest request:
                        HandleLobbyLeaveRequest(request);
                        break;
                    case MatchCreateRequest request:
                        HandleMatchCreateRequest(request);
                        break;
                    case ServerStarted serverStarted:
                        HandleServerStarted(serverStarted);
                        break;
                    case MatchEndRequest request:
                        HandleMatchEndRequest(request);
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleMatchEndRequest(MatchEndRequest request)
        {
            // Find the lobby
            Room room = _rooms.FirstOrDefault(r => r.RoomCode == request.RoomCode);
            // MatchEndResponse to players in lobby
            MatchEndResponse matchEndResponse = new MatchEndResponse {ResponseCode = ResponseCode.Ok};
            // Scene Change to the players in the lobby
            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};
            // Panel Change to the players in the lobby
            PanelChange panelChange = new PanelChange {PanelToChangeTo = "LobbyPanelFromGame"};
            
            foreach (Client player in room.Players)
            {
                // Get the KeyValuePair
                KeyValuePair<Client, TcpClient> pair =
                    _connectedPlayers.FirstOrDefault(c => c.Key.Id == player.Id);
                // Send lobby data
                SendObject(pair, matchEndResponse);
                SendObject(pair, sceneChange);
                //SendObject(pair, panelChange);
            }
            
            // Stop the process
            room.gameInstance.Kill();
            // Remove the process from the room
            room.gameInstance = new Process();
            // Remove game instance client from the connected clients list
            Client serverClient = _connectedPlayers.FirstOrDefault(c => c.Key.Id == request.ServerId).Key;
            room.Server = new Client();
            _connectedPlayers.Remove(serverClient);
        }
        
        private void HandleServerStarted(ServerStarted serverStarted)
        {
            Room room = _rooms.FirstOrDefault(r => r.RoomCode == serverStarted.GameInstanceRoomCode);

            if (room == null) return;

            MatchCreateResponse matchCreateResponse = new MatchCreateResponse
                {MatchPortNumber = serverStarted.GameInstancePort, ResponseCode = ResponseCode.Ok};

            PlayerStateChangeResponse playerStateChangeResponse = new PlayerStateChangeResponse
                {NewPlayerState = PlayerState.InGame};

            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "GameScene"};
            
            foreach (Client player in room.Players)
            {
                KeyValuePair<Client, TcpClient> clientPair =
                    _connectedPlayers.FirstOrDefault(c => c.Key.Id == player.Id);

                clientPair.Key.PlayerState = PlayerState.InGame;
                
                SendObject(clientPair, matchCreateResponse);
                SendObject(clientPair, playerStateChangeResponse);
                SendObject(clientPair, sceneChange);
            }
        }

        private void HandleMatchCreateRequest(MatchCreateRequest request)
        {
            // Grab an available port from the ports list.
            int port = _ports[0];
            _ports.Remove(port);
            // Start a new game process.
            Room room = _rooms.FirstOrDefault(r => r.RoomCode == request.RoomCode);

            Process gameInstance = new Process
            {
                StartInfo =
                {
                    FileName =
                        "C:\\Repositories\\InternetAdventures\\InternetAdventures\\Builds\\Networked\\V1.0.5S\\InternetAdventures.exe",
                    Arguments = $"-server {port} {request.RoomCode}",
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true
                }
            };

            if (room != null) room.gameInstance = gameInstance;

            gameInstance.Start();
        }
        
        private void HandleLobbyLeaveRequest(LobbyLeaveRequest request)
        {
            KeyValuePair<Client, TcpClient> clientPair =
                _connectedPlayers.FirstOrDefault(c => c.Key.Id == request.RequestedPlayerId);

            Room room = _rooms.FirstOrDefault(r => r.RoomCode == clientPair.Key.JoinedRoomCode);

            if (room == null) return;

            if (room.Players.Count == 2)
            {
                room.Players.Remove(clientPair.Key);
                
                KeyValuePair<Client, TcpClient> secondClient =
                    _connectedPlayers.FirstOrDefault(c => c.Key.Id == room.Players[0].Id);
                
                if (clientPair.Key.IsLobbyLeader)
                {
                    Log.LogInfo($"Giving Player with ID {secondClient.Key.Id} lobby leader status", this, ConsoleColor.Magenta);
                    secondClient.Key.IsLobbyLeader = true;
                    clientPair.Key.IsLobbyLeader = false;
                    room.IsMatchmakingAllowed = false;
                }
                
                LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                    {Lobby = room, ResponseCode = ResponseCode.Ok};
                SendObject(secondClient, lobbyDataResponse);
            }
            else if (room.Players.Count == 1)
            {
                // Remove player from room
                room.Players.Remove(clientPair.Key);
                // Delete room from list
                Log.LogInfo($"Removing room with code: {room.RoomCode}", this, ConsoleColor.Magenta);
                _rooms.Remove(room);

                clientPair.Key.IsLobbyLeader = false;
            }
            
            // Send player back to Host/Join panel
            clientPair.Key.ReadyState = ReadyState.NotReady;
            clientPair.Key.PlayerState = PlayerState.SearchingForLobby;

            PlayerStateChangeResponse playerStateChangeResponse = new PlayerStateChangeResponse
                {NewPlayerState = PlayerState.SearchingForLobby, PlayerId = clientPair.Key.Id};
            SendObject(clientPair, playerStateChangeResponse);

            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "MainMenu"};
            SendObject(clientPair, sceneChange);
            
            PanelChange panelChange = new PanelChange {PanelToChangeTo = "JoinHostPanel"};
            SendObject(clientPair, panelChange);

            LobbyLeaveResponse lobbyLeaveResponse = new LobbyLeaveResponse {ResponseCode = ResponseCode.Ok};
            SendObject(clientPair, lobbyLeaveResponse);
        }
        
        private void HandleReadyStateChangeRequest(ReadyStateChangeRequest request)
        {
            KeyValuePair<Client, TcpClient> clientPair =
                _connectedPlayers.FirstOrDefault(c => c.Key.Id == request.RequestingPlayerId);

            switch (clientPair.Key.ReadyState)
            {
                case ReadyState.Ready:
                    clientPair.Key.ReadyState = ReadyState.NotReady;
                    break;
                case ReadyState.NotReady:
                    clientPair.Key.ReadyState = ReadyState.Ready;
                    break;
                default:
                    break;
            }
            
            // Get the room the player is in.
            Room room = _rooms.Find(r => r.RoomCode == clientPair.Key.JoinedRoomCode);

            if (room == null) return;

            int playersReady = 0;
            
            foreach (Client player in room.Players)
            {
                if (player.ReadyState == ReadyState.Ready)
                {
                    playersReady += 1;
                }
            }

            room.IsMatchmakingAllowed = playersReady == 2;

            // Send lobby data to both clients that are connected to the lobby
            LobbyDataResponse lobbyDataResponse = new LobbyDataResponse {Lobby = room, ResponseCode = ResponseCode.Ok};

            foreach (Client player in room.Players)
            {
                // Get the KeyValuePair
                KeyValuePair<Client, TcpClient> pair =
                    _connectedPlayers.FirstOrDefault(c => c.Key.Id == player.Id);
                // Send lobby data
                SendObject(pair, lobbyDataResponse);
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

                clientPair.Key.JoinedRoomCode = request.RoomCode;
                
                _rooms.Find(r => r.RoomCode == request.RoomCode)?.Players.Add(clientPair.Key);

                SendObject(clientPair, lobbyJoinResponse);

                LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                    {Lobby = _rooms.Find(r => r.RoomCode == request.RoomCode), ResponseCode = ResponseCode.Ok};

                KeyValuePair<Client, TcpClient> otherClientPair =
                    _connectedPlayers.FirstOrDefault(c => c.Key.Id == room.Players[0].Id);
                
                SendObject(otherClientPair, lobbyDataResponse);

                // Tell the client to switch to the lobby panel
                PanelChange panelChange = new PanelChange {PanelToChangeTo = "LobbyPanel"};
                SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};
                SendObject(clientPair, sceneChange);
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
            clientPair.Key.JoinedRoomCode = roomCode;
            clientPair.Key.IsLobbyLeader = true;
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
            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};
            SendObject(clientPair, sceneChange);

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

            switch (client.ClientType)
            {
                // Since this method is only called when a client is connected
                // Send a PanelChange packet
                case ClientType.Client:
                {
                    PanelChange panelChange = new PanelChange { PanelToChangeTo = "MainPanel" };
                    Log.LogInfo($"Sending: {panelChange}", this, ConsoleColor.DarkBlue);
                    SendObject(clientPair, panelChange);
                    break;
                }
                case ClientType.GameInstance:
                {
                    StartServerInstance serverInstance = new StartServerInstance();
                    Log.LogInfo($"Sending: {serverInstance}", this, ConsoleColor.DarkBlue);
                    SendObject(clientPair, serverInstance);
                    break;   
                }
            }
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