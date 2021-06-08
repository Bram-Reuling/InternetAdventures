using Shared.model;

namespace Shared.protocol
{
    public class PlayerStateChangeRequest : ISerializable
    {
        public int PlayerId { get; set; } = 0;
        public PlayerState RequestedPlayerState { get; set; } = PlayerState.InLobby;
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(PlayerId);
            pPacket.Write((int)RequestedPlayerState);
        }

        public void Deserialize(Packet pPacket)
        {
            PlayerId = pPacket.ReadInt();
            RequestedPlayerState = (PlayerState)pPacket.ReadInt();
        }
    }
}