using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using shared;
using Shared;
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
        //private Dictionary<Client, TcpClient> _connectedPlayers = new Dictionary<Client, TcpClient>();
        private List<ClientServerInfo> _connectedPlayers = new List<ClientServerInfo>();
        private List<Room> _rooms = new List<Room>();
        private List<int> _ports = new List<int> {55556, 55557, 55558, 55559};
        private List<ClientServerInfo> faultyClients = new List<ClientServerInfo>();

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
                Console.WriteLine("Server started on port 55555");

                _listener = new TcpListener(IPAddress.Any, 55555);
                _listener.Start();
                
                IPEndPoint localEndPoint = _listener.Server.LocalEndPoint as IPEndPoint;
                IPEndPoint remoteEndPoint = _listener.Server.RemoteEndPoint as IPEndPoint;

                if (localEndPoint != null) Console.WriteLine($"Started on Local IP: {localEndPoint.Address}");
                if (remoteEndPoint != null) Console.WriteLine($"Started on Remote IP: {remoteEndPoint.Address}");

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
                    Console.WriteLine("New client pending!");
                    Client newClient = new Client();
                    GeneratePlayerId(ref newClient);

                    Log.LogInfo("Accepted new client.", this, ConsoleColor.Green);
                    Console.WriteLine("Accepted new client.");
                    TcpClient tcpClient = _listener.AcceptTcpClient();

                    ClientServerInfo clientServerInfo = new ClientServerInfo();
                    clientServerInfo.SetClient(newClient);
                    clientServerInfo.SetTcpClient(tcpClient);
                    clientServerInfo.SetLastIsAliveTime(DateTime.Now);
                    
                    _connectedPlayers.Add(clientServerInfo);

                    ClientDataRequest playerDataRequest = new ClientDataRequest {Client = newClient};

                    SendObject(clientServerInfo, playerDataRequest);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ProcessExistingClients()
        {
            foreach (ClientServerInfo player in _connectedPlayers)
            {
                if (player.TcpClient.Available == 0) continue;

                byte[] inBytes = StreamUtil.Read(player.TcpClient.GetStream());
                Packet inPacket = new Packet(inBytes);
                ISerializable inObject = inPacket.ReadObject();

                Log.LogInfo($"Received: {inObject}", this, ConsoleColor.Blue);
                Console.WriteLine($"Received: {inObject}");

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
                    case IsAlive isAlive:
                        HandleIsAlivePacket(player);
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleIsAlivePacket(ClientServerInfo clientServerInfo)
        {
            clientServerInfo.SetLastIsAliveTime(DateTime.Now);
        }
        
        private void HandleMatchEndRequest(MatchEndRequest request)
        {
            // Find the lobby
            Room room = _rooms.FirstOrDefault(r => r.RoomCode == request.RoomCode);
            // MatchEndResponse to players in lobby
            MatchEndResponse matchEndResponse = new MatchEndResponse {ResponseCode = ResponseCode.Ok};
            // Scene Change to the players in the lobby
            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};

            foreach (Client player in room.Players)
            {
                // Get the KeyValuePair
                ClientServerInfo clientServerInfo =
                    _connectedPlayers.FirstOrDefault(c => c.Client.Id == player.Id);
                // Send lobby data
                SendObject(clientServerInfo, matchEndResponse);
                SendObject(clientServerInfo, sceneChange);
            }
            
            _ports.Add(room.Port);
            room.Port = 0;
            
            // Stop the process
            room.gameInstance.Kill();
            // Remove the process from the room
            room.gameInstance = new Process();
            // Remove game instance client from the connected clients list
            ClientServerInfo serverClient = _connectedPlayers.FirstOrDefault(c => c.Client.Id == request.ServerId);
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
                ClientServerInfo clientServerInfo =
                    _connectedPlayers.FirstOrDefault(c => c.Client.Id == player.Id);

                if (clientServerInfo != null) clientServerInfo.Client.PlayerState = PlayerState.InGame;

                SendObject(clientServerInfo, matchCreateResponse);
                SendObject(clientServerInfo, playerStateChangeResponse);
                SendObject(clientServerInfo, sceneChange);
            }
        }

        private void HandleMatchCreateRequest(MatchCreateRequest request)
        {
            // Grab an available port from the ports list.
            int port = _ports[0];
            _ports.Remove(port);
            // Start a new game process.
            Room room = _rooms.FirstOrDefault(r => r.RoomCode == request.RoomCode);

            //TODO: make it debian compatible
            Process gameInstance = new Process
            {
                StartInfo =
                {
                    FileName =
                        "../LinuxServer/InternetAdventuresServer.x86_64",
                    Arguments = $"-server {port} {request.RoomCode}",
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true
                }
            };

            if (room != null)
            {
                room.gameInstance = gameInstance;
                room.Port = port;
            }

            gameInstance.Start();
        }
        
        private void HandleLobbyLeaveRequest(LobbyLeaveRequest request)
        {
            ClientServerInfo clientServerInfo =
                _connectedPlayers.FirstOrDefault(c => c.Client.Id == request.RequestedPlayerId);

            if (clientServerInfo == null) return;
            
            Room room = _rooms.FirstOrDefault(r => r.RoomCode == clientServerInfo.Client.JoinedRoomCode);

            if (room == null) return;

            if (room.Players.Count == 2)
            {
                room.Players.Remove(clientServerInfo.Client);
                
                ClientServerInfo secondClient =
                    _connectedPlayers.FirstOrDefault(c => c.Client.Id == room.Players[0].Id);
                
                if (clientServerInfo.Client.IsLobbyLeader)
                {
                    Log.LogInfo($"Giving Player with ID {secondClient.Client.Id} lobby leader status", this, ConsoleColor.Magenta);
                    Console.WriteLine($"Giving Player with ID {secondClient.Client.Id} lobby leader status");
                    secondClient.Client.IsLobbyLeader = true;
                    clientServerInfo.Client.IsLobbyLeader = false;
                    room.IsMatchmakingAllowed = false;
                }
                
                LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                    {Lobby = room, ResponseCode = ResponseCode.Ok};
                SendObject(secondClient, lobbyDataResponse);
            }
            else if (room.Players.Count == 1)
            {
                // Remove player from room
                room.Players.Remove(clientServerInfo.Client);
                // Delete room from list
                Log.LogInfo($"Removing room with code: {room.RoomCode}", this, ConsoleColor.Magenta);
                Console.WriteLine($"Removing room with code: {room.RoomCode}");
                _rooms.Remove(room);

                clientServerInfo.Client.IsLobbyLeader = false;
            }
            
            // Send player back to Host/Join panel
            clientServerInfo.Client.ReadyState = ReadyState.NotReady;
            clientServerInfo.Client.PlayerState = PlayerState.SearchingForLobby;

            PlayerStateChangeResponse playerStateChangeResponse = new PlayerStateChangeResponse
                {NewPlayerState = PlayerState.SearchingForLobby, PlayerId = clientServerInfo.Client.Id};
            SendObject(clientServerInfo, playerStateChangeResponse);

            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "MainMenu"};
            SendObject(clientServerInfo, sceneChange);
            
            PanelChange panelChange = new PanelChange {PanelToChangeTo = "JoinHostPanel"};
            SendObject(clientServerInfo, panelChange);

            LobbyLeaveResponse lobbyLeaveResponse = new LobbyLeaveResponse {ResponseCode = ResponseCode.Ok};
            SendObject(clientServerInfo, lobbyLeaveResponse);
        }
        
        private void HandleReadyStateChangeRequest(ReadyStateChangeRequest request)
        {
            ClientServerInfo clientServerInfo =
                _connectedPlayers.FirstOrDefault(c => c.Client.Id == request.RequestingPlayerId);

            if (clientServerInfo == null) return;
            
            switch (clientServerInfo.Client.ReadyState)
            {
                case ReadyState.Ready:
                    clientServerInfo.Client.ReadyState = ReadyState.NotReady;
                    break;
                case ReadyState.NotReady:
                    clientServerInfo.Client.ReadyState = ReadyState.Ready;
                    break;
                default:
                    break;
            }
            
            // Get the room the player is in.
            Room room = _rooms.Find(r => r.RoomCode == clientServerInfo.Client.JoinedRoomCode);

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
                ClientServerInfo serverInfo =
                    _connectedPlayers.FirstOrDefault(c => c.Client.Id == player.Id);
                // Send lobby data
                SendObject(serverInfo, lobbyDataResponse);
            }
        }
        
        private void HandleLobbyJoinRequest(LobbyJoinRequest request)
        {
            // Check if room with room code exists
            Room room = _rooms.Find(r => r.RoomCode == request.RoomCode);

            ClientServerInfo clientServerInfo =
                _connectedPlayers.FirstOrDefault(c => c.Client.Id == request.RequestingPlayerId);

            if (room == null)
            {
                Log.LogInfo($"Lobby not found with code: {request.RoomCode}!", this, ConsoleColor.Red);
                Console.WriteLine($"Lobby not found with code: {request.RoomCode}!");
                LobbyJoinResponse lobbyJoinResponse = new LobbyJoinResponse
                    {ResponseCode = ResponseCode.Error, ResponseMessage = "No room found with room code!"};
                
                SendObject(clientServerInfo, lobbyJoinResponse);
            }
            else
            {
                Log.LogInfo("Lobby found!", this, ConsoleColor.DarkGreen);
                LobbyJoinResponse lobbyJoinResponse = new LobbyJoinResponse
                    {ResponseCode = ResponseCode.Ok, RoomCode = request.RoomCode};

                clientServerInfo.Client.JoinedRoomCode = request.RoomCode;
                
                _rooms.Find(r => r.RoomCode == request.RoomCode)?.Players.Add(clientServerInfo.Client);

                SendObject(clientServerInfo, lobbyJoinResponse);

                LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                    {Lobby = _rooms.Find(r => r.RoomCode == request.RoomCode), ResponseCode = ResponseCode.Ok};

                ClientServerInfo otherClientServerInfo =
                    _connectedPlayers.FirstOrDefault(c => c.Client.Id == room.Players[0].Id);
                
                SendObject(otherClientServerInfo, lobbyDataResponse);

                // Tell the client to switch to the lobby panel
                PanelChange panelChange = new PanelChange {PanelToChangeTo = "LobbyPanel"};
                SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};
                SendObject(clientServerInfo, sceneChange);
            }
        }
        
        private void HandleLobbyDataRequest(LobbyDataRequest request)
        {
            Room room = _rooms.Find(p => p.RoomCode == request.RoomCode);

            if (room == null) return;
            
            LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                {Lobby = room, ResponseCode = ResponseCode.Ok};
                
            ClientServerInfo clientServerInfo =
                _connectedPlayers.FirstOrDefault(c => c.Client.Id == request.RequestingPlayerId);
                
            SendObject(clientServerInfo, lobbyDataResponse);
        }
        
        private void HandleLobbyCreateRequest(LobbyCreateRequest request)
        {
            // Generate room code
            string roomCode = GenerateRoomCode();
            Log.LogInfo($"Generated RoomCode: {roomCode}", this, ConsoleColor.Magenta);
            Console.WriteLine($"Generated RoomCode: {roomCode}");
            // Create room
            Room room = new Room { Id = _rooms.Count + 1, RoomCode = roomCode };
            Log.LogInfo("Created a new room!", this, ConsoleColor.Green);
            Console.WriteLine("Created a new room!");
            // Add user to room
            ClientServerInfo clientServerInfo =
                _connectedPlayers.FirstOrDefault(c => c.Client.Id == request.RequestingPlayerId);
            clientServerInfo.Client.JoinedRoomCode = roomCode;
            clientServerInfo.Client.IsLobbyLeader = true;
            room.Players.Add(clientServerInfo.Client);
            Log.LogInfo("Added the requesting player to the players list", this, ConsoleColor.Green);
            Console.WriteLine("Added the requesting player to the players list");
            // Add room to the room list
            _rooms.Add(room);
            Log.LogInfo("Added the room to the rooms list with ID: " + room.Id, this, ConsoleColor.Green);
            Console.WriteLine("Added the room to the rooms list with ID: " + room.Id);
            // Send LobbyCreateResponse
            LobbyCreateResponse lobbyCreateResponse = new LobbyCreateResponse
                {ResponseCode = ResponseCode.Ok, RoomCode = roomCode};
            
            SendObject(clientServerInfo, lobbyCreateResponse);
            
            // Tell the client to switch to the lobby panel
            PanelChange panelChange = new PanelChange {PanelToChangeTo = "LobbyPanel"};
            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};
            SendObject(clientServerInfo, sceneChange);

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
            ClientServerInfo clientServerInfo =
                _connectedPlayers.FirstOrDefault(p => p.Client.Id == response.Client.Id);
            Client client = clientServerInfo.Client;
            Log.LogInfo($"Client Name: {response.Client.Name}", this, ConsoleColor.Blue);
            Console.WriteLine($"Client Name: {response.Client.Name}");
            client.Name = response.Client.Name;
            Log.LogInfo($"Client Type: {response.Client.ClientType}", this, ConsoleColor.Blue);
            Console.WriteLine($"Client Type: {response.Client.ClientType}");
            client.ClientType = response.Client.ClientType;

            switch (client.ClientType)
            {
                // Since this method is only called when a client is connected
                // Send a PanelChange packet
                case ClientType.Client:
                {
                    PanelChange panelChange = new PanelChange { PanelToChangeTo = "MainPanel" };
                    Log.LogInfo($"Sending: {panelChange}", this, ConsoleColor.DarkBlue);
                    Console.WriteLine($"Sending: {panelChange}");
                    SendObject(clientServerInfo, panelChange);
                    break;
                }
                case ClientType.GameInstance:
                {
                    StartServerInstance serverInstance = new StartServerInstance();
                    Log.LogInfo($"Sending: {serverInstance}", this, ConsoleColor.DarkBlue);
                    Console.WriteLine($"Sending: {serverInstance}");
                    SendObject(clientServerInfo, serverInstance);
                    break;   
                }
            }
        }

        private void HandlePlayerStateChangeRequest(PlayerStateChangeRequest request)
        {
            // Secure this
            ClientServerInfo clientServerInfo =
                _connectedPlayers.FirstOrDefault(c => c.Client.Id == request.PlayerId);
            
            PlayerStateChangeResponse playerStateChangeResponse = new PlayerStateChangeResponse
                {NewPlayerState = request.RequestedPlayerState, PlayerId = request.PlayerId};
            
            SendObject(clientServerInfo, playerStateChangeResponse);
        }
        
        private void ProcessFaultyClients()
        {
            foreach (ClientServerInfo player in _connectedPlayers)
            {
                TimeSpan difference = DateTime.Now.Subtract(player.LastIsAliveTime);

                if (difference.Seconds <= 7) continue;
                if (!faultyClients.Contains(player))
                    faultyClients.Add(player);
            }
            
            // Process the faulty clients list
            foreach (ClientServerInfo faultyClient in faultyClients)
            {
                faultyClient.TcpClient.Close();
                _connectedPlayers.Remove(faultyClient);
            }
            
            faultyClients.Clear();
        }

        private void GeneratePlayerId(ref Client playerObject)
        {
            var rnd = new Random();
            int id = 0;
            bool result = true;

            while (result)
            {
                id = rnd.Next(10001);
                result = _connectedPlayers.Any(p => p.Client.Id == id);
            }

            playerObject.Id = id;
        }

        private void RemovePlayer(ClientServerInfo player)
        {
            try
            {
                faultyClients.Add(player);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SendObject(ClientServerInfo player, ISerializable outObject)
        {
            try
            {
                Log.LogInfo($"Sending {outObject} to player with ID: {player.Client.Id}", this, ConsoleColor.Cyan);
                Console.WriteLine($"Sending {outObject} to player with ID: {player.Client.Id}");
                Packet outPacket = new Packet();
                outPacket.Write(outObject);
                
                StreamUtil.Write(player.TcpClient.GetStream(), outPacket.GetBytes());
            }
            catch (Exception e)
            {
                // Remove the faulty client
                //Console.WriteLine(e);
                Log.LogInfo("Removing faulty client!", this, ConsoleColor.Red);
                Console.WriteLine("Removing faulty client!");
                if (!player.TcpClient.Connected)
                {
                    RemovePlayer(player);
                }
            }
        }
    }
}