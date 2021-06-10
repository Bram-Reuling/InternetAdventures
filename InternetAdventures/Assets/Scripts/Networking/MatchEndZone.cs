using System;
using Mirror;
using UnityEngine;

namespace Networking
{
    [RequireComponent(typeof(BoxCollider))]
    public class MatchEndZone : NetworkBehaviour
    {
        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Character"))
            {
                Debug.Log("Character entered trigger");
                EventBroker.CallPlayerEnterMatchEndZoneEvent();
            }
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Character"))
            {
                Debug.Log("Character exited trigger");
                EventBroker.CallPlayerExitMatchEndZoneEvent();
            }
        }
    }
}