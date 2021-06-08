namespace Shared.protocol.Match
{
    public class MatchEndRequest : ISerializable
    {
        public string RoomCode { get; set; } = "00000";

        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RoomCode);
        }

        public void Deserialize(Packet pPacket)
        {
            RoomCode = pPacket.ReadString();
        }
    }
}