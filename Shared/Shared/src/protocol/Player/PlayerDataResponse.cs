using Shared.model;

namespace Shared.protocol
{
    public class PlayerDataResponse : ISerializable
    {
        public Player Player { get; set; } = new Player();
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(Player);
        }

        public void Deserialize(Packet pPacket)
        {
            Player = pPacket.Read<Player>();
        }
    }
}