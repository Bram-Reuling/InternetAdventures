namespace Shared.protocol
{
    public class PanelChange : ISerializable
    {
        public string PanelToChangeTo { get; set; } = "";
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(PanelToChangeTo);
        }

        public void Deserialize(Packet pPacket)
        {
            PanelToChangeTo = pPacket.ReadString();
        }
    }
}