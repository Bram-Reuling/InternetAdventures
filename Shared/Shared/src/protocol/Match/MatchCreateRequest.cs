using Shared.model;

namespace Shared.protocol.Match
{
    public class MatchCreateRequest : ISerializable
    {
        public int RequestingPlayerId { get; set; } = 0;
        public Room Lobby { get; set; } = new Room();
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RequestingPlayerId);
            pPacket.Write(Lobby);
        }

        public void Deserialize(Packet pPacket)
        {
            RequestingPlayerId = pPacket.ReadInt();
            Lobby = pPacket.Read<Room>();
        }
    }
}