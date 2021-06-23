using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MainServer.PacketHandlers;
using shared;
using Shared;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Lobby;
using Shared.protocol.Match;
using Shared.protocol.protocol;

namespace MainServer
{
    public class Server
    {
        private TcpListener listener;
        public List<ClientServerInfo> ConnectedPlayers { get; } = new List<ClientServerInfo>();
        public List<Room> Rooms { get; } = new List<Room>();
        public List<int> Ports { get; } = new List<int> {55556, 55557, 55558, 55559};
        private List<ClientServerInfo> faultyClients = new List<ClientServerInfo>();

        // Handlers
        private AlivePacketsHandler alivePacketsHandler;
        private MatchPacketsHandler matchPacketsHandler;
        private ClientPacketsHandler clientPacketsHandler;
        private ServerPacketsHandler serverPacketsHandler;
        private LobbyPacketsHandler lobbyPacketsHandler;

        public static void Main(string[] args)
        {
            Server server = new Server();
            server.RunServer();
        }

        private void InitializeHandlers()
        {
            alivePacketsHandler = new AlivePacketsHandler();
            matchPacketsHandler = new MatchPacketsHandler(this);
            clientPacketsHandler = new ClientPacketsHandler(this);
            serverPacketsHandler = new ServerPacketsHandler(this);
            lobbyPacketsHandler = new LobbyPacketsHandler(this);
        }

        private void RunServer()
        {
            try
            {
                InitializeHandlers();
                Log.LogInfo("Server starting on port 55555", this, ConsoleColor.White);
                Console.WriteLine("Server started on port 55555");

                listener = new TcpListener(IPAddress.Any, 55555);
                listener.Start();

                IPEndPoint localEndPoint = listener.Server.LocalEndPoint as IPEndPoint;
                IPEndPoint remoteEndPoint = listener.Server.RemoteEndPoint as IPEndPoint;

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

        #region Process Functions

        private void ProcessNewClients()
        {
            try
            {
                while (listener.Pending())
                {
                    Log.LogInfo("New client pending!", this, ConsoleColor.Green);
                    Console.WriteLine("New client pending!");
                    Client newClient = new Client();
                    GeneratePlayerId(ref newClient);

                    Log.LogInfo("Accepted new client.", this, ConsoleColor.Green);
                    Console.WriteLine("Accepted new client.");
                    TcpClient tcpClient = listener.AcceptTcpClient();

                    ClientServerInfo clientServerInfo = new ClientServerInfo();
                    clientServerInfo.SetClient(newClient);
                    clientServerInfo.SetTcpClient(tcpClient);
                    clientServerInfo.SetLastIsAliveTime(DateTime.Now);

                    ConnectedPlayers.Add(clientServerInfo);

                    ClientDataRequest playerDataRequest = new ClientDataRequest {Client = newClient};

                    SendPacketToClient(clientServerInfo, playerDataRequest);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ProcessExistingClients()
        {
            foreach (ClientServerInfo player in ConnectedPlayers)
            {
                if (player.TcpClient.Available == 0) continue;

                byte[] inBytes = StreamUtil.Read(player.TcpClient.GetStream());
                Packet inPacket = new Packet(inBytes);
                ISerializable inObject = inPacket.ReadObject();

                Log.LogInfo($"Received: {inObject}", this, ConsoleColor.Blue);
                Console.WriteLine($"Received: {inObject}");

                HandlePacket(inObject, player);
            }
        }

        private void ProcessFaultyClients()
        {
            // Check the amount of time between alive packets of a client
            // if the amount of time is bigger than 7 seconds, add the client to the
            // faulty client list for removal
            foreach (ClientServerInfo player in ConnectedPlayers)
            {
                TimeSpan difference = DateTime.Now.Subtract(player.LastIsAliveTime);

                if (difference.Seconds <= 20) continue;

                if (!faultyClients.Contains(player))
                {
                    Console.WriteLine("Adding player to faulty clients!");
                    faultyClients.Add(player);
                }
            }

            // Process the faulty clients list
            foreach (ClientServerInfo faultyClient in faultyClients)
            {
                Console.WriteLine("Removing player!");

                // Check the client state
                if (faultyClient.Client.PlayerState is PlayerState.InLobby or PlayerState.InGame)
                {
                    // if the client state is in lobby or game, send
                    // LobbyDataResponse to the other player.   
                    
                    // Get the room the player is in
                    Room room = GetRoom(faultyClient.Client.JoinedRoomCode);
                    if (room != null)
                    {
                        room.Players.Remove(faultyClient.Client);
                        
                        foreach (Client player in room.Players)
                        {
                            if (player.Id != faultyClient.Client.Id)
                            {
                                ClientServerInfo otherPlayer = GetClientServerInfo(player.Id);

                                LobbyDataResponse lobbyDataResponse = new LobbyDataResponse {Lobby = room};
                                
                                SendPacketToClient(otherPlayer, lobbyDataResponse);
                            }
                        }
                    }
                }

                faultyClient.TcpClient.Close();
                ConnectedPlayers.Remove(faultyClient);
            }

            if (faultyClients.Count > 0)
                faultyClients.Clear();
        }

        #endregion

        #region Main Handler

        private void HandlePacket(ISerializable inObject, ClientServerInfo player)
        {
            switch (inObject)
            {
                case ClientDataResponse response:
                    clientPacketsHandler.HandleClientDataResponse(response);
                    break;
                case PlayerStateChangeRequest request:
                    clientPacketsHandler.HandleClientStateChangeRequest(request);
                    break;
                case LobbyCreateRequest request:
                    lobbyPacketsHandler.HandleLobbyCreateRequest(request);
                    break;
                case LobbyDataRequest request:
                    lobbyPacketsHandler.HandleLobbyDataRequest(request);
                    break;
                case LobbyJoinRequest request:
                    lobbyPacketsHandler.HandleLobbyJoinRequest(request);
                    break;
                case ReadyStateChangeRequest request:
                    clientPacketsHandler.HandleClientReadyStateChangeRequest(request);
                    break;
                case LobbyLeaveRequest request:
                    lobbyPacketsHandler.HandleLobbyLeaveRequest(request);
                    break;
                case MatchCreateRequest request:
                    matchPacketsHandler.HandleMatchCreateRequest(request);
                    break;
                case ServerStarted serverStarted:
                    serverPacketsHandler.HandleServerStarted(serverStarted);
                    break;
                case MatchEndRequest request:
                    matchPacketsHandler.HandleMatchEndRequest(request);
                    break;
                case IsAlive isAlive:
                    alivePacketsHandler.HandleIsAlivePacket(player);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Utils

        public string GenerateRoomCode()
        {
            Random rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code = "";
            bool result = true;

            while (result)
            {
                code = new string(Enumerable.Repeat(chars, 5).Select(s => s[rnd.Next(s.Length)]).ToArray());
                result = Rooms.Any(c => c.RoomCode == code);
            }

            return code;
        }

        public void GeneratePlayerId(ref Client playerObject)
        {
            var rnd = new Random();
            int id = 0;
            bool result = true;

            while (result)
            {
                id = rnd.Next(10001);
                result = ConnectedPlayers.Any(p => p.Client.Id == id);
            }

            playerObject.Id = id;
        }

        public void QueuePlayerForRemoval(ClientServerInfo player)
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

        public void SendPacketToClient(ClientServerInfo player, ISerializable outObject)
        {
            try
            {
                Console.WriteLine($"Sending {outObject} to player with ID: {player.Client.Id}");
                Packet outPacket = new Packet();
                outPacket.Write(outObject);

                StreamUtil.Write(player.TcpClient.GetStream(), outPacket.GetBytes());
            }
            catch (Exception e)
            {
                // Remove the faulty clients
                Console.WriteLine("Removing faulty client!");
                if (!player.TcpClient.Connected)
                {
                    QueuePlayerForRemoval(player);
                }
            }
        }

        public Room GetRoom(string pRoomCode)
        {
            return Rooms.FirstOrDefault(r => r.RoomCode == pRoomCode);
        }

        public ClientServerInfo GetClientServerInfo(int pClientId)
        {
            return ConnectedPlayers.FirstOrDefault(c => c.Client.Id == pClientId);
        }

        #endregion
    }
}