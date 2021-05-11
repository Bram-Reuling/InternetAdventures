using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NPC
{
    public class CommunityMember : MonoBehaviour
    {
        public int minimumLookRange = 10;

        private List<GameObject> players;
        private ChatBubble chatBubble;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            chatBubble = GetComponent<ChatBubble>();
        }

        private void Update()
        {
        }
    }
}
