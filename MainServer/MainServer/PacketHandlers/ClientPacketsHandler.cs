using System;
using System.Diagnostics.CodeAnalysis;
using shared;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Lobby;
using Shared.protocol.Match;

namespace MainServer.PacketHandlers
{
    public class ClientPacketsHandler
    {
        private readonly Server serverInstance;

        public ClientPacketsHandler([NotNull] Server pServerInstance)
        {
            serverInstance = pServerInstance;
        }

        public void HandleClientDataResponse(ClientDataResponse response)
        {
            ClientServerInfo clientServerInfo = serverInstance.GetClientServerInfo(response.Client.Id);
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
                    PanelChange panelChange = new PanelChange {PanelToChangeTo = "MainPanel"};
                    Log.LogInfo($"Sending: {panelChange}", this, ConsoleColor.DarkBlue);
                    Console.WriteLine($"Sending: {panelChange}");
                    serverInstance.SendPacketToClient(clientServerInfo, panelChange);
                    break;
                }
                case ClientType.GameInstance:
                {
                    StartServerInstance gameServerInstance = new StartServerInstance();
                    Log.LogInfo($"Sending: {gameServerInstance}", this, ConsoleColor.DarkBlue);
                    Console.WriteLine($"Sending: {gameServerInstance}");
                    serverInstance.SendPacketToClient(clientServerInfo, gameServerInstance);
                    break;
                }
            }
        }

        public void HandleClientStateChangeRequest(PlayerStateChangeRequest request)
        {
            // Secure this
            ClientServerInfo clientServerInfo = serverInstance.GetClientServerInfo(request.PlayerId);

            PlayerStateChangeResponse playerStateChangeResponse = new PlayerStateChangeResponse
                {NewPlayerState = request.RequestedPlayerState, PlayerId = request.PlayerId};

            serverInstance.SendPacketToClient(clientServerInfo, playerStateChangeResponse);
        }

        public void HandleClientReadyStateChangeRequest(ReadyStateChangeRequest request)
        {
            ClientServerInfo clientServerInfo = serverInstance.GetClientServerInfo(request.RequestingPlayerId);

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
            Room room = serverInstance.GetRoom(clientServerInfo.Client.JoinedRoomCode);

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
                ClientServerInfo serverInfo = serverInstance.GetClientServerInfo(player.Id);
                if (serverInfo == null) continue;
                // Send lobby data
                serverInstance.SendPacketToClient(serverInfo, lobbyDataResponse);
            }
        }
    }
}