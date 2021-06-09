using System.Collections.Generic;
using System.Diagnostics;

namespace Shared.model
{
    public class Room : ISerializable
    {
        public int Id { get; set; } = 0;
        public string RoomCode { get; set; } = "00000";
        public RoomState RoomState { get; set; } = RoomState.Lobby;
        public List<Client> Players { get; set; } = new List<Client>();
        public Client Server { get; set; } = new Client();
        public Process gameInstance { get; set; } = new Process();
        public bool IsMatchmakingAllowed = false;

        public Room()
        {
            
        }
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(Id);
            pPacket.Write(RoomCode);
            pPacket.Write((int)RoomState);

            pPacket.Write(Players.Count);

            foreach (Client player in Players)
            {
                pPacket.Write(player);
            }
            
            pPacket.Write(Server);
            pPacket.Write(IsMatchmakingAllowed);
        }

        public void Deserialize(Packet pPacket)
        {
            Id = pPacket.ReadInt();
            RoomCode = pPacket.ReadString();
            RoomState = (RoomState)pPacket.ReadInt();

            int playerCount = pPacket.ReadInt();

            for (int i = 0; i < playerCount; i++)
            {
                Players.Add(pPacket.Read<Client>());
            }

            Server = pPacket.Read<Client>();
            IsMatchmakingAllowed = pPacket.ReadBool();
        }
    }
}