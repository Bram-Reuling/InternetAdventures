using Shared.model;

namespace Shared.protocol.Match
{
    public class MatchCreateRequest : ISerializable
    {
        public int RequestingPlayerId { get; set; } = 0;
        public string RoomCode { get; set; } = "00000";
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RequestingPlayerId);
            pPacket.Write(RoomCode);
        }

        public void Deserialize(Packet pPacket)
        {
            RequestingPlayerId = pPacket.ReadInt();
            RoomCode = pPacket.ReadString();
        }
    }
}