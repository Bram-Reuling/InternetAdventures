using Mirror;
using UnityEngine;

namespace Networking.NPC
{
    public class NetworkCommunityMember : NetworkBehaviour
    {
        #region Variables

        private NetworkChatBubble chatBubble;

        #endregion

        #region Global Functions

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            chatBubble = GetComponent<NetworkChatBubble>();
        }

        #endregion

        #region Client Functions

        #endregion

        #region Server Functions

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Character")) return;
            chatBubble.EnableEmojis();
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Character")) return;
            chatBubble.DisableEmojis();
        }

        #endregion
    }
}