namespace Shared
{
    public interface ISerializable
    {
        void Serialize(Packet pPacket);
        void Deserialize(Packet pPacket);
    }
}