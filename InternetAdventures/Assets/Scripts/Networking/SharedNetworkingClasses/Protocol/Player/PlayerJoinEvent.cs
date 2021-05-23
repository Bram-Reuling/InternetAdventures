namespace Networking
{
    public class PlayerJoinEvent : ASerializable
    {
        public PlayerInfo playerToAdd;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(playerToAdd);
        }

        public override void Deserialize(Packet pPacket)
        {
            playerToAdd = pPacket.Read<PlayerInfo>();
        }
    }
}