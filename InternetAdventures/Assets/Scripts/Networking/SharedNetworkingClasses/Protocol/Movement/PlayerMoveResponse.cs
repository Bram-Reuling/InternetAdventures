using UnityEngine;

namespace Networking
{
    public class PlayerMoveResponse : ASerializable
    {
        public PlayerInfo player;
        
        public Vector3 inputPosition;
        public Quaternion inputRotation;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(player);
            
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
            player = pPacket.Read<PlayerInfo>();
            
            inputPosition = new Vector3 {x = pPacket.ReadFloat(), y = pPacket.ReadFloat(), z = pPacket.ReadFloat()};

            inputRotation = new Quaternion { w = pPacket.ReadFloat(), x = pPacket.ReadFloat(), y = pPacket.ReadFloat(), z = pPacket.ReadFloat() };
        }
    }
}