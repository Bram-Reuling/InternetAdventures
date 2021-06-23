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

        private int numberOfPlayers = 0;
        private bool calledEvent = false;
        
        [ServerCallback]
        private void Update()
        {
            if (!calledEvent && numberOfPlayers == 2)
            {
                Debug.Log("Calling Scene Change Event");
                EventBroker.CallSceneChangeEvent(LevelName);
                calledEvent = true;
            }
        }

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
                    Debug.Log("Adding player");
                    numberOfPlayers++;
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
                else
                {
                    Debug.Log("Removing player");
                    numberOfPlayers--;
                }
            }
        }
    }
}