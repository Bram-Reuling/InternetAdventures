namespace Shared
{
    public class PlayerInfo : ASerializable
    {
        public SVector3 position;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(position.X);
            pPacket.Write(position.Y);
            pPacket.Write(position.Z);
        }

        public override void Deserialize(Packet pPacket)
        {
            position = new SVector3();

            position.X = pPacket.ReadFloat();
            position.Y = pPacket.ReadFloat();
            position.Z = pPacket.ReadFloat();
        }
    }
}