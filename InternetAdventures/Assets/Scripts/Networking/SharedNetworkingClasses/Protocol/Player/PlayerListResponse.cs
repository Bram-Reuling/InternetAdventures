using System.Collections.Generic;

namespace Networking
{
    public class PlayerListResponse : ASerializable
    {
        public List<PlayerInfo> playerList;
        
        public override void Serialize(Packet pPacket)
        {
            int count = playerList?.Count ?? 0;
            
            pPacket.Write(count);
            
            for (int i = 0; i < count; i++)
            {
                if (playerList != null) pPacket.Write(playerList[i]);
            }
        }

        public override void Deserialize(Packet pPacket)
        {
            playerList = new List<PlayerInfo>();
            
            int count = pPacket.ReadInt();
            
            for (int i = 0; i < count; i++)
            {
                playerList.Add(pPacket.Read<PlayerInfo>());
            }
        }
    }
}