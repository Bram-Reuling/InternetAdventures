namespace Networking
{
    public class ClientHeartbeat : ASerializable
    {
        public int randomNumber = 0;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(randomNumber);
        }

        public override void Deserialize(Packet pPacket)
        {
            randomNumber = pPacket.ReadInt();
        }
    }
}