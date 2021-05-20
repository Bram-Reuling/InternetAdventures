using UnityEngine;

namespace Networking
{
    public class NetworkPlayerInformation : MonoBehaviour
    {
        public int playerId;

        public void SetPlayerId(int value)
        {
            playerId = value;
        }
    }
}