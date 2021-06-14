namespace Shared.protocol.Lobby
{
    public class LobbyDataRequest : ISerializable
    {
        public int RequestingPlayerId { get; set; } = 0;
        public string RoomCode { get; set; } = "";

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