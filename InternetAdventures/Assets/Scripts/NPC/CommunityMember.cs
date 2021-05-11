using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NPC
{
    public class CommunityMember : MonoBehaviour
    {
        public float minimumTriggerSpan = 10;

        private float minimumTriggerRadius;

        private List<GameObject> players;
        private ChatBubble chatBubble;

        public GameObject cameraRig;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            chatBubble = GetComponent<ChatBubble>();

            minimumTriggerRadius = minimumTriggerSpan;
        }

        private void Update()
        {
            Vector3 averagePositionBetweenPlayers = players.Aggregate(Vector3.one, (current, player) => current + player.transform.position);

            averagePositionBetweenPlayers /= players.Count;

            float distanceBetweenNpcAndPlayer = Vector3.Distance(transform.position, averagePositionBetweenPlayers);

            if (distanceBetweenNpcAndPlayer <= minimumTriggerRadius)
            {
                // Trigger the chat bubble stuffs
                chatBubble.EnableEmojis();
            }
            else
            {
                // Disable the chat bubble stuffs
                chatBubble.DisableEmojis();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, minimumTriggerSpan);
        }
    }
}
