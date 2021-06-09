using Shared.model;

namespace Shared.protocol.Lobby
{
    public class LobbyLeaveResponse : ISerializable
    {

        public ResponseCode ResponseCode { get; set; } = ResponseCode.Ok;
        public string ResponseMessage { get; set; } = "";
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write((int)ResponseCode);
            pPacket.Write(ResponseMessage);
        }

        public void Deserialize(Packet pPacket)
        {
            ResponseCode = (ResponseCode) pPacket.ReadInt();
            ResponseMessage = pPacket.ReadString();
        }
    }
}