using System;
using System.Diagnostics.CodeAnalysis;
using shared;
using Shared.model;
using Shared.protocol;
using Shared.protocol.Lobby;
using Shared.protocol.protocol;

namespace MainServer.PacketHandlers
{
    public class LobbyPacketsHandler
    {
        private readonly Server serverInstance;

        public LobbyPacketsHandler([NotNull] Server pServerInstance)
        {
            serverInstance = pServerInstance;
        }

        public void HandleLobbyLeaveRequest(LobbyLeaveRequest request)
        {
            ClientServerInfo clientServerInfo = serverInstance.GetClientServerInfo(request.RequestedPlayerId);

            if (clientServerInfo == null) return;

            Room room = serverInstance.GetRoom(clientServerInfo.Client.JoinedRoomCode);

            if (room == null) return;

            if (room.Players.Count == 2)
            {
                room.Players.Remove(clientServerInfo.Client);

                ClientServerInfo secondClient =
                    serverInstance.GetClientServerInfo(room.Players[0].Id);

                if (clientServerInfo.Client.IsLobbyLeader)
                {
                    Log.LogInfo($"Giving Player with ID {secondClient.Client.Id} lobby leader status", this,
                        ConsoleColor.Magenta);
                    Console.WriteLine($"Giving Player with ID {secondClient.Client.Id} lobby leader status");
                    secondClient.Client.IsLobbyLeader = true;
                    clientServerInfo.Client.IsLobbyLeader = false;
                    room.IsMatchmakingAllowed = false;
                }

                LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                    {Lobby = room, ResponseCode = ResponseCode.Ok};
                serverInstance.SendPacketToClient(secondClient, lobbyDataResponse);
            }
            else if (room.Players.Count == 1)
            {
                // Remove player from room
                room.Players.Remove(clientServerInfo.Client);
                // Delete room from list
                Log.LogInfo($"Removing room with code: {room.RoomCode}", this, ConsoleColor.Magenta);
                Console.WriteLine($"Removing room with code: {room.RoomCode}");
                serverInstance.Rooms.Remove(room);

                clientServerInfo.Client.IsLobbyLeader = false;
            }

            // Send player back to Host/Join panel
            clientServerInfo.Client.ReadyState = ReadyState.NotReady;
            clientServerInfo.Client.PlayerState = PlayerState.SearchingForLobby;

            PlayerStateChangeResponse playerStateChangeResponse = new PlayerStateChangeResponse
                {NewPlayerState = PlayerState.SearchingForLobby, PlayerId = clientServerInfo.Client.Id};
            serverInstance.SendPacketToClient(clientServerInfo, playerStateChangeResponse);

            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "MainMenu"};
            serverInstance.SendPacketToClient(clientServerInfo, sceneChange);

            PanelChange panelChange = new PanelChange {PanelToChangeTo = "JoinHostPanel"};
            serverInstance.SendPacketToClient(clientServerInfo, panelChange);

            LobbyLeaveResponse lobbyLeaveResponse = new LobbyLeaveResponse {ResponseCode = ResponseCode.Ok};
            serverInstance.SendPacketToClient(clientServerInfo, lobbyLeaveResponse);
        }

        public void HandleLobbyJoinRequest(LobbyJoinRequest request)
        {
            // Check if room with room code exists
            Room room = serverInstance.GetRoom(request.RoomCode);

            ClientServerInfo clientServerInfo = serverInstance.GetClientServerInfo(request.RequestingPlayerId);

            if (clientServerInfo == null) return;

            if (room == null)
            {
                Log.LogInfo($"Lobby not found with code: {request.RoomCode}!", this, ConsoleColor.Red);
                Console.WriteLine($"Lobby not found with code: {request.RoomCode}!");
                LobbyJoinResponse lobbyJoinResponse = new LobbyJoinResponse
                    {ResponseCode = ResponseCode.Error, ResponseMessage = "No room found with room code!"};

                serverInstance.SendPacketToClient(clientServerInfo, lobbyJoinResponse);
            }
            else
            {
                Log.LogInfo("Lobby found!", this, ConsoleColor.DarkGreen);
                LobbyJoinResponse lobbyJoinResponse = new LobbyJoinResponse
                    {ResponseCode = ResponseCode.Ok, RoomCode = request.RoomCode};

                clientServerInfo.Client.JoinedRoomCode = request.RoomCode;

                serverInstance.GetRoom(request.RoomCode)?.Players.Add(clientServerInfo.Client);

                serverInstance.SendPacketToClient(clientServerInfo, lobbyJoinResponse);

                LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                    {Lobby = serverInstance.GetRoom(request.RoomCode), ResponseCode = ResponseCode.Ok};

                ClientServerInfo otherClientServerInfo = serverInstance.GetClientServerInfo(room.Players[0].Id);

                serverInstance.SendPacketToClient(otherClientServerInfo, lobbyDataResponse);

                // Tell the client to switch to the lobby panel
                SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};
                serverInstance.SendPacketToClient(clientServerInfo, sceneChange);
            }
        }

        public void HandleLobbyDataRequest(LobbyDataRequest request)
        {
            Room room = serverInstance.GetRoom(request.RoomCode);

            if (room == null) return;

            LobbyDataResponse lobbyDataResponse = new LobbyDataResponse
                {Lobby = room, ResponseCode = ResponseCode.Ok};

            ClientServerInfo clientServerInfo = serverInstance.GetClientServerInfo(request.RequestingPlayerId);

            if (clientServerInfo == null) return;

            serverInstance.SendPacketToClient(clientServerInfo, lobbyDataResponse);
        }

        public void HandleLobbyCreateRequest(LobbyCreateRequest request)
        {
            // Generate room code
            string roomCode = serverInstance.GenerateRoomCode();
            Log.LogInfo($"Generated RoomCode: {roomCode}", this, ConsoleColor.Magenta);
            Console.WriteLine($"Generated RoomCode: {roomCode}");

            // Create room
            Room room = new Room {Id = serverInstance.Rooms.Count + 1, RoomCode = roomCode};
            Log.LogInfo("Created a new room!", this, ConsoleColor.Green);
            Console.WriteLine("Created a new room!");

            // Add user to room
            ClientServerInfo clientServerInfo = serverInstance.GetClientServerInfo(request.RequestingPlayerId);

            // if the user is not found, just cancel the creation process
            if (clientServerInfo == null) return;

            clientServerInfo.Client.JoinedRoomCode = roomCode;
            clientServerInfo.Client.IsLobbyLeader = true;
            room.Players.Add(clientServerInfo.Client);
            Log.LogInfo("Added the requesting player to the players list", this, ConsoleColor.Green);
            Console.WriteLine("Added the requesting player to the players list");

            // Add room to the room list
            serverInstance.Rooms.Add(room);
            Log.LogInfo("Added the room to the rooms list with ID: " + room.Id, this, ConsoleColor.Green);
            Console.WriteLine("Added the room to the rooms list with ID: " + room.Id);

            // Send LobbyCreateResponse
            LobbyCreateResponse lobbyCreateResponse = new LobbyCreateResponse
                {ResponseCode = ResponseCode.Ok, RoomCode = roomCode};

            serverInstance.SendPacketToClient(clientServerInfo, lobbyCreateResponse);

            // Tell the client to switch to the lobby scene
            SceneChange sceneChange = new SceneChange {SceneToSwitchTo = "LobbyMenu"};
            serverInstance.SendPacketToClient(clientServerInfo, sceneChange);
        }
    }
}