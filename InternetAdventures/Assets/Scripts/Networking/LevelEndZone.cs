using System;
using Mirror;
using UnityEngine;

namespace Networking
{
    [RequireComponent(typeof(BoxCollider))]
    public class LevelEndZone : NetworkBehaviour
    {
        [SerializeField] private bool SwitchToNewLevel = true;
        [Scene, SerializeField] private string LevelName = "";
        
        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Character"))
            {
                if (!SwitchToNewLevel)
                {
                    Debug.Log("Character entered trigger");
                    EventBroker.CallPlayerEnterMatchEndZoneEvent();   
                }
                else
                {
                    EventBroker.CallSceneChangeEvent(LevelName);
                }
            }
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Character"))
            {
                if (!SwitchToNewLevel)
                {
                    Debug.Log("Character exited trigger");
                    EventBroker.CallPlayerExitMatchEndZoneEvent();   
                }
            }
        }
    }
}