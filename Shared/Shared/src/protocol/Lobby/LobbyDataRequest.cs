namespace Shared.protocol.Lobby
{
    public class LobbyDataRequest : ISerializable
    {
        public int RequestingPlayerId { get; set; } = 0;

        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RequestingPlayerId);
        }

        public void Deserialize(Packet pPacket)
        {
            RequestingPlayerId = pPacket.ReadInt();
        }
    }
}