using Shared.model;

namespace Shared.protocol
{
    public class ClientDataResponse : ISerializable
    {
        public Client Client { get; set; } = new Client();
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(Client);
        }

        public void Deserialize(Packet pPacket)
        {
            Client = pPacket.Read<Client>();
        }
    }
}