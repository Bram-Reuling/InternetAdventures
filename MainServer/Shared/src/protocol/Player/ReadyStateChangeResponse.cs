using Shared.model;

namespace Shared.protocol
{
    public class ReadyStateChangeResponse : ISerializable
    {
        public ReadyState NewReadyState { get; set; } = ReadyState.NotReady;
        public ResponseCode ResponseCode { get; set; } = ResponseCode.Ok;
        public string ResponseMessage { get; set; } = "";
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write((int)NewReadyState);
            pPacket.Write((int)ResponseCode);
            pPacket.Write(ResponseMessage);
        }

        public void Deserialize(Packet pPacket)
        {
            NewReadyState = (ReadyState) pPacket.ReadInt();
            ResponseCode = (ResponseCode) pPacket.ReadInt();
            ResponseMessage = pPacket.ReadString();
        }
    }
}