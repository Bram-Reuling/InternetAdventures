using System.Collections.Generic;

namespace Shared
{
    public class PlayerListUpdateEvent : ASerializable
    {
        public PlayerListUpdateType updateType;
        public List<PlayerInfo> updatedPlayerList;

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write((int)updateType);
            
            int count = updatedPlayerList?.Count ?? 0;
            
            pPacket.Write(count);
            
            for (int i = 0; i < count; i++)
            {
                if (updatedPlayerList != null) pPacket.Write(updatedPlayerList[i]);
            }
        }

        public override void Deserialize(Packet pPacket)
        {
            updateType = (PlayerListUpdateType)pPacket.ReadInt();
            
            updatedPlayerList = new List<PlayerInfo>();
            
            int count = pPacket.ReadInt();
            
            for (int i = 0; i < count; i++)
            {
                updatedPlayerList.Add(pPacket.Read<PlayerInfo>());
            }
        }
    }
}