using Shared.model;

namespace Shared.protocol.Lobby
{
    public class LobbyCreateResponse : ISerializable
    {
        public string RoomCode { get; set; } = "00000";
        public ResponseCode ResponseCode { get; set; } = ResponseCode.Ok;
        public string ResponseMessage { get; set; } = "";
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(RoomCode);
            pPacket.Write((int)ResponseCode);
            pPacket.Write(ResponseMessage);
        }

        public void Deserialize(Packet pPacket)
        {
            RoomCode = pPacket.ReadString();
            ResponseCode = (ResponseCode) pPacket.ReadInt();
            ResponseMessage = pPacket.ReadString();
        }
    }
}