using Mirror;
using Networking.Character;
using UnityEngine;

namespace Networking.CheckPointSystem
{
    public class NetworkCheckpoint : NetworkBehaviour
    {
        private GameObject firstPlayer;
        
        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Character"))
            {
                if (firstPlayer == null || other.gameObject == firstPlayer)
                {
                    firstPlayer = other.gameObject;
                    return;
                }
                
                
                firstPlayer.GetComponent<NetworkPlayerLogic>().SetCheckPoint(transform.position);
                other.gameObject.GetComponent<NetworkPlayerLogic>().SetCheckPoint(transform.position);
            }
        }
    }
}