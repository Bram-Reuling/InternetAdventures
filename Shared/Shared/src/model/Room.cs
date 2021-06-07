using System.Collections.Generic;

namespace Shared.model
{
    public class Room : ISerializable
    {
        public int Id { get; set; } = 0;
        public string RoomCode { get; set; } = "00000";
        public RoomState RoomState { get; set; } = RoomState.Lobby;
        public List<Player> Players { get; set; } = new List<Player>();

        public Room()
        {
            
        }
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(Id);
            pPacket.Write(RoomCode);
            pPacket.Write((int)RoomState);

            pPacket.Write(Players.Count);

            foreach (Player player in Players)
            {
                pPacket.Write(player);
            }
        }

        public void Deserialize(Packet pPacket)
        {
            Id = pPacket.ReadInt();
            RoomCode = pPacket.ReadString();
            RoomState = (RoomState)pPacket.ReadInt();

            int playerCount = pPacket.ReadInt();

            for (int i = 0; i < playerCount; i++)
            {
                Players.Add(pPacket.Read<Player>());
            }
        }
    }
}