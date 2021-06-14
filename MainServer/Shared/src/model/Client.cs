namespace Shared.model
{
    public class Client : ISerializable
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "Name";
        public PlayerState PlayerState { get; set; } = PlayerState.SearchingForLobby;
        public ReadyState ReadyState { get; set; } = ReadyState.NotReady;
        public ClientType ClientType { get; set; } = ClientType.Client;
        public string JoinedRoomCode { get; set; } = "00000";
        public bool IsLobbyLeader { get; set; } = false;

        public Client()
        {
            
        }
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(Id);
            pPacket.Write(Name);
            pPacket.Write((int)PlayerState);
            pPacket.Write((int)ReadyState);
            pPacket.Write((int)ClientType);
            pPacket.Write(JoinedRoomCode);
            pPacket.Write(IsLobbyLeader);
        }

        public void Deserialize(Packet pPacket)
        {
            Id = pPacket.ReadInt();
            Name = pPacket.ReadString();
            PlayerState = (PlayerState)pPacket.ReadInt();
            ReadyState = (ReadyState)pPacket.ReadInt();
            ClientType = (ClientType)pPacket.ReadInt();
            JoinedRoomCode = pPacket.ReadString();
            IsLobbyLeader = pPacket.ReadBool();
        }
    }
}