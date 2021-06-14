using Shared.model;

namespace Shared.protocol.Lobby
{
    public class LobbyDataResponse : ISerializable
    {
        public Room Lobby { get; set; } = new Room();
        public ResponseCode ResponseCode { get; set; } = ResponseCode.Ok;
        public string ResponseMessage { get; set; } = "";
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(Lobby);
            pPacket.Write((int)ResponseCode);
            pPacket.Write(ResponseMessage);
        }

        public void Deserialize(Packet pPacket)
        {
            Lobby = pPacket.Read<Room>();
            ResponseCode = (ResponseCode) pPacket.ReadInt();
            ResponseMessage = pPacket.ReadString();
        }
    }
}