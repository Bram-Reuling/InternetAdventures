using System.Numerics;

namespace Shared
{
    public class PlayerMoveRequest : ASerializable
    {
        public Vector2 input;
        
        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(input.X);
            pPacket.Write(input.Y);
        }

        public override void Deserialize(Packet pPacket)
        {
            input = new Vector2 { X = pPacket.ReadFloat(), Y = pPacket.ReadFloat() };
        }
    }
}