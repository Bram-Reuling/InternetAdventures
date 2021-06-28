using System.Diagnostics.CodeAnalysis;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Lobby;
using Shared.protocol.Match;
using Shared.protocol.protocol;

namespace MainServer.PacketHandlers
{
    public class ServerPacketsHandler
    {
        private readonly Server serverInstance;

        public ServerPacketsHandler([NotNull]Server pServerInstance)
        {
            serverInstance = pServerInstance;
        }
        
        public void HandleServerStarted(ServerStarted serverStarted)
        {
            Room room = serverInstance.GetRoom(serverStarted.GameInstanceRoomCode);

            if (room == null) return;

            MatchCreateResponse matchCreateResponse = new MatchCreateResponse
                {MatchPortNumber = serverStarted.GameInstancePort, ResponseCode = ResponseCode.Ok};

            PlayerStateChangeResponse playerStateChangeResponse = new PlayerStateChangeResponse
                {NewPlayerState = PlayerState.InGame};

            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "GameScene"};

            foreach (Client player in room.Players)
            {
                ClientServerInfo clientServerInfo =
                    serverInstance.GetClientServerInfo(player.Id);

                if (clientServerInfo != null) clientServerInfo.Client.PlayerState = PlayerState.InGame;

                serverInstance.SendPacketToClient(clientServerInfo, matchCreateResponse);
                serverInstance.SendPacketToClient(clientServerInfo, playerStateChangeResponse);
                serverInstance.SendPacketToClient(clientServerInfo, sceneChange);
            }
            
            // Send the players details to the server.
            // Used for displaying the players names.

            LobbyDataResponse lobbyDataResponse = new LobbyDataResponse {Lobby = room};

            ClientServerInfo serverInstanceInfo = serverInstance.GetClientServerInfo(room.Server.Id);
            
            serverInstance.SendPacketToClient(serverInstanceInfo, lobbyDataResponse);
        }
    }
}