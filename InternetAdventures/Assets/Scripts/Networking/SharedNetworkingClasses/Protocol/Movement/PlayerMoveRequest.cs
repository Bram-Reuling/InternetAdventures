using UnityEngine;

namespace Networking
{
    public class PlayerMoveRequest : ASerializable
    {
        public int playerID;
        
        public Vector3 inputPosition;
        public Quaternion inputRotation;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(playerID);
            
            pPacket.Write(inputPosition.x);
            pPacket.Write(inputPosition.y);
            pPacket.Write(inputPosition.z);
            
            pPacket.Write(inputRotation.w);
            pPacket.Write(inputRotation.x);
            pPacket.Write(inputRotation.y);
            pPacket.Write(inputRotation.z);
        }

        public override void Deserialize(Packet pPacket)
        {
            playerID = pPacket.ReadInt();
            
            inputPosition = new Vector3 {x = pPacket.ReadFloat(), y = pPacket.ReadFloat(), z = pPacket.ReadFloat()};

            inputRotation = new Quaternion { w = pPacket.ReadFloat(), x = pPacket.ReadFloat(), y = pPacket.ReadFloat(), z = pPacket.ReadFloat() };
        }
    }
}
