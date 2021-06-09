namespace Shared.protocol.Lobby
{
    public class LobbyLeaveRequest : ISerializable
    {
        public int RequestedPlayerId { get; set; } = 0;
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RequestedPlayerId);
        }

        public void Deserialize(Packet pPacket)
        {
            RequestedPlayerId = pPacket.ReadInt();
        }
    }
}