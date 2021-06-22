using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkInteractableTrigger : NetworkBehaviour
{
    [SerializeField] private InteractableEnum Interactable;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Character"))
        {
            other.transform.GetComponent<NetworkInteractableHandler>().UnlockInteractable(Interactable);
        }
    }
}
