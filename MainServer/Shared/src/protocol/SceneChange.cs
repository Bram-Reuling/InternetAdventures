namespace Shared.protocol.protocol
{
    public class SceneChange : ISerializable
    {
        public string SceneToSwitchTo { get; set; } = "";
        
        public void Serialize(Packet pPacket)
        {
            pPacket.Write(SceneToSwitchTo);
        }

        public void Deserialize(Packet pPacket)
        {
            SceneToSwitchTo = pPacket.ReadString();
        }
    }
}