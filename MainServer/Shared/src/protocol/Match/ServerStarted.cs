namespace Shared.protocol.Match
{
    public class ServerStarted : ISerializable
    {
        public int GameInstanceClientId { get; set; } = 0;
        public string GameInstanceRoomCode { get; set; } = "";
        public int GameInstancePort { get; set; } = 0;
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(GameInstanceClientId);
            pPacket.Write(GameInstanceRoomCode);
            pPacket.Write(GameInstancePort);
        }

        public void Deserialize(Packet pPacket)
        {
            GameInstanceClientId = pPacket.ReadInt();
            GameInstanceRoomCode = pPacket.ReadString();
            GameInstancePort = pPacket.ReadInt();
        }
    }
}