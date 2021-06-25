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
            StartBubble();
        }

        private void Init()
        {
            chatBubble = GetComponent<NetworkChatBubble>();
        }

        [ServerCallback]
        private void StartBubble()
        {
            chatBubble.EnableEmojis();
        }

        #endregion

        #region Client Functions

        #endregion

        #region Server Functions

        #endregion
    }
}