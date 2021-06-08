using Shared.model;

namespace Shared.protocol
{
    public class PlayerStateChangeResponse : ISerializable
    {
        public int PlayerId { get; set; } = 0;
        public PlayerState NewPlayerState = PlayerState.InLobby;
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(PlayerId);
            pPacket.Write((int)NewPlayerState);
        }

        public void Deserialize(Packet pPacket)
        {
            PlayerId = pPacket.ReadInt();
            NewPlayerState = (PlayerState)pPacket.ReadInt();
        }
    }
}