using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Shared.model;
using Shared.protocol.Match;
using Shared.protocol.protocol;

namespace MainServer.PacketHandlers
{
    public class MatchPacketsHandler
    {
        private readonly Server serverInstance;

        public MatchPacketsHandler([NotNull] Server pServerInstance)
        {
            serverInstance = pServerInstance;
        }

        public void HandleMatchCreateRequest(MatchCreateRequest request)
        {
            // Grab an available port from the ports list.
            int port = serverInstance.Ports[0];
            serverInstance.Ports.Remove(port);
            // Start a new game process.
            Room room = serverInstance.GetRoom(request.RoomCode);

            // Create new game server instance
            Process gameInstance = new Process
            {
                StartInfo =
                {
                    FileName = "../LinuxServer/InternetAdventuresServer.x86_64",
                    //FileName = "C:\\Repositories\\InternetAdventures\\InternetAdventures\\Builds\\Individual Presentation\\Game Server\\InternetAdventures.exe",
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
        
        public void HandleMatchEndRequest(MatchEndRequest request)
        {
            // Find the lobby
            Room room = serverInstance.GetRoom(request.RoomCode);

            if (room == null) return;

            // MatchEndResponse to players in lobby
            MatchEndResponse matchEndResponse = new MatchEndResponse {ResponseCode = ResponseCode.Ok};
            // Scene Change to the players in the lobby
            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};

            foreach (Client player in room.Players)
            {
                // Get the KeyValuePair
                ClientServerInfo clientServerInfo =
                    serverInstance.GetClientServerInfo(player.Id);
                // Send lobby data
                serverInstance.SendPacketToClient(clientServerInfo, matchEndResponse);
                serverInstance.SendPacketToClient(clientServerInfo, sceneChange);
            }

            serverInstance.Ports.Add(room.Port);
            room.Port = 0;

            // Stop the process
            room.gameInstance.Kill();
            // Remove the process from the room
            room.gameInstance = new Process();
            // Remove game instance client from the connected clients list
            ClientServerInfo serverClient = serverInstance.GetClientServerInfo(request.ServerId);
            if (serverClient == null) return;
            room.Server = new Client();
            serverInstance.QueuePlayerForRemoval(serverClient);
        }
    }
}