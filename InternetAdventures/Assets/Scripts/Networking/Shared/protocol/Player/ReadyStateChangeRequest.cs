using Shared.model;

namespace Shared.protocol
{
    public class ReadyStateChangeRequest : ISerializable
    {
        public int RequestingPlayerId { get; set; } = 0;
        public ReadyState RequestedReadyState { get; set; } = ReadyState.NotReady;
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RequestingPlayerId);
            pPacket.Write((int)RequestedReadyState);
        }

        public void Deserialize(Packet pPacket)
        {
            RequestingPlayerId = pPacket.ReadInt();
            RequestedReadyState = (ReadyState) pPacket.ReadInt();
        }
    }
}