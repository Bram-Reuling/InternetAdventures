namespace Shared.protocol.Lobby
{
    public class LobbyCreateRequest : ISerializable
    {
        public int RequestingPlayerId { get; set; } = 0;
        public string LobbyDescription { get; set; } = "";
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RequestingPlayerId);
            pPacket.Write(LobbyDescription);
        }

        public void Deserialize(Packet pPacket)
        {
            RequestingPlayerId = pPacket.ReadInt();
            LobbyDescription = pPacket.ReadString();
        }
    }
}