using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkInteractableTrigger : NetworkBehaviour
{
    [SerializeField] private InteractableEnum Interactable;
    [SerializeField] private GameObject pickUpInteractable;
    private int timesPickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Character"))
        {
            other.transform.GetComponent<NetworkInteractableHandler>().UnlockInteractable(Interactable);
            timesPickedUp++;
            if (timesPickedUp > 1)
            {
                Destroy(pickUpInteractable);
                Destroy(gameObject);
            }
        }
    }
}
