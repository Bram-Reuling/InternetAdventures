using System.Collections.Generic;

namespace Shared
{
    public class PlayerJoinedEvent : ASerializable
    {
        public List<PlayerInfo> players;

        public int justANumber;
        
        public override void Serialize(Packet pPacket)
        {
            int count = players?.Count ?? 0;
            
            pPacket.Write(count);
            
            for (int i = 0; i < count; i++)
            {
                if (players != null) pPacket.Write(players[i]);
            }
            
            //pPacket.Write(justANumber);
        }

        public override void Deserialize(Packet pPacket)
        {
            players = new List<PlayerInfo>();
            
            int count = pPacket.ReadInt();
            
            for (int i = 0; i < count; i++)
            {
                players.Add(pPacket.Read<PlayerInfo>());
            }

            //justANumber = pPacket.ReadInt();
        }
    }
}