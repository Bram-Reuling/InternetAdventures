namespace Shared.model
{
    public class Player : ISerializable
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "Name";
        public PlayerState PlayerState { get; set; } = PlayerState.SearchingForLobby;

        public Player()
        {
            
        }
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(Id);
            pPacket.Write(Name);
            pPacket.Write((int)PlayerState);
        }

        public void Deserialize(Packet pPacket)
        {
            Id = pPacket.ReadInt();
            Name = pPacket.ReadString();
            PlayerState = (PlayerState)pPacket.ReadInt();
        }
    }
}