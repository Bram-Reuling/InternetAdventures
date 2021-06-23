using System;
using Mirror;
using UnityEngine;

namespace Networking.NPC
{
    [RequireComponent(typeof(BoxCollider))]
    public class ScammerCheckPoint : NetworkBehaviour
    {
        [SerializeField] private GameObject scammer;
        [SerializeField] private GameObject scammerSpawnPoint;

        private bool alreadyUsed = false;

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (!alreadyUsed && other.CompareTag("Character"))
            {
                scammer.transform.position = scammerSpawnPoint.transform.position;
                alreadyUsed = true;
            }
        }
    }
}