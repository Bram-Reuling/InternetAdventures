namespace Shared.protocol.Match
{
    public class MatchEndRequest : ISerializable
    {
        public string RoomCode { get; set; } = "00000";
        public int ServerId { get; set; } = 0;

        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RoomCode);
            pPacket.Write(ServerId);
        }

        public void Deserialize(Packet pPacket)
        {
            RoomCode = pPacket.ReadString();
            ServerId = pPacket.ReadInt();
        }
    }
}