using Mirror;
using Networking.Character;
using UnityEngine;

namespace Networking.CheckPointSystem
{
    public class NetworkCheckpoint : NetworkBehaviour
    {
        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            // Get NetworkPlayerLogic from collider
            NetworkPlayerLogic playerLogic = other.GetComponent<NetworkPlayerLogic>();

            // Check if the component is there.
            if (playerLogic)
            {
                // If it is call the SetCheckpoint method
                playerLogic.SetCheckPoint(transform.position);
            }
        }
    }
}