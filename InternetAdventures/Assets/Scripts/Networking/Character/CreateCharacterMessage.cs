using Mirror;

namespace Networking.Character
{
    public struct CreateCharacterMessage : NetworkMessage
    {
        public int skinIndex;
    }
}