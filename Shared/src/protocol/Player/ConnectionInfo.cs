namespace Shared
{
    public class ConnectionInfo : ASerializable
    {
        public int ID;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(ID);
        }

        public override void Deserialize(Packet pPacket)
        {
            ID = pPacket.ReadInt();
        }
    }
}