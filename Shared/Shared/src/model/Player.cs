namespace Shared.model
{
    public class Player : ISerializable
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "Name";
        public PlayerState PlayerState { get; set; } = PlayerState.SearchingForLobby;
        public ReadyState ReadyState { get; set; } = ReadyState.NotReady;

        public Player()
        {
            
        }
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(Id);
            pPacket.Write(Name);
            pPacket.Write((int)PlayerState);
            pPacket.Write((int)ReadyState);
        }

        public void Deserialize(Packet pPacket)
        {
            Id = pPacket.ReadInt();
            Name = pPacket.ReadString();
            PlayerState = (PlayerState)pPacket.ReadInt();
            ReadyState = (ReadyState)pPacket.ReadInt();
        }
    }
}