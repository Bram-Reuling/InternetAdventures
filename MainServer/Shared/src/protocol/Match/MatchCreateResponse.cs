using Shared.model;

namespace Shared.protocol.Match
{
    public class MatchCreateResponse : ISerializable
    {
        public int MatchPortNumber { get; set; } = 0;
        public ResponseCode ResponseCode { get; set; } = ResponseCode.Ok;
        public string ResponseMessage { get; set; } = ""; 
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(MatchPortNumber);
            pPacket.Write((int)ResponseCode);
            pPacket.Write(ResponseMessage);
        }

        public void Deserialize(Packet pPacket)
        {
            MatchPortNumber = pPacket.ReadInt();
            ResponseCode = (ResponseCode) pPacket.ReadInt();
            ResponseMessage = pPacket.ReadString();
        }
    }
}