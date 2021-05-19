using System.Numerics;
using System.Reflection;

namespace Shared
{
    public class PlayerInfo : ASerializable
    {
        public int ID;
        public Vector3 position;
        public Quaternion rotation;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(ID);
            
            pPacket.Write(position.X);
            pPacket.Write(position.Y);
            pPacket.Write(position.Z);
            
            pPacket.Write(rotation.W);
            pPacket.Write(rotation.X);
            pPacket.Write(rotation.Y);
            pPacket.Write(rotation.Z);
        }

        public override void Deserialize(Packet pPacket)
        {
            ID = pPacket.ReadInt();

            position = new Vector3 {X = pPacket.ReadFloat(), Y = pPacket.ReadFloat(), Z = pPacket.ReadFloat()};

            rotation = new Quaternion
                {W = pPacket.ReadFloat(), X = pPacket.ReadFloat(), Y = pPacket.ReadFloat(), Z = pPacket.ReadFloat()};

        }
    }
}