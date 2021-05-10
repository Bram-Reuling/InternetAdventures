using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NPC
{
    public class CommunityMember : MonoBehaviour
    {
        public int minimumLookRange = 10;

        private List<GameObject> players;

        private void Start()
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
        }

        private void Update()
        {
        }
    }
}
