namespace Networking
{
    public class PlayerRemoveEvent : ASerializable
    {
        public PlayerInfo playerToRemove;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(playerToRemove);
        }

        public override void Deserialize(Packet pPacket)
        {
            playerToRemove = pPacket.Read<PlayerInfo>();
        }
    }
}