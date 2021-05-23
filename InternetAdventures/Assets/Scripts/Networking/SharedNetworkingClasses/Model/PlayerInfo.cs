using UnityEngine;
namespace Networking
{
    public class PlayerInfo : ASerializable
    {
        public int ID;
        public Vector3 position;
        public Quaternion rotation;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(ID);
            
            pPacket.Write(position.x);
            pPacket.Write(position.y);
            pPacket.Write(position.z);
            
            pPacket.Write(rotation.w);
            pPacket.Write(rotation.x);
            pPacket.Write(rotation.y);
            pPacket.Write(rotation.z);
        }

        public override void Deserialize(Packet pPacket)
        {
            ID = pPacket.ReadInt();

            position = new Vector3 {x = pPacket.ReadFloat(), y = pPacket.ReadFloat(), z = pPacket.ReadFloat()};

            rotation = new Quaternion
                {w = pPacket.ReadFloat(), x = pPacket.ReadFloat(), y = pPacket.ReadFloat(), z = pPacket.ReadFloat()};

        }
    }
}