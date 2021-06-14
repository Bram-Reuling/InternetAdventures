using Mirror;
using Networking.Character;
using UnityEngine;

namespace Networking.CheckPointSystem
{
    public class NetworkVoidVolume : NetworkBehaviour
    {
        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            // Get NetworkPlayerLogic from collider
            NetworkPlayerLogic playerLogic = other.GetComponent<NetworkPlayerLogic>();

            if (playerLogic)
            {
                playerLogic.RespawnPlayer();
            }
        }
    }
}