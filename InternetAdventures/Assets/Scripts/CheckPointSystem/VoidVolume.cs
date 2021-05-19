using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            Debug.Log("Player Collided with Void Volume");
            EventBroker.CallRespawnCharacterEvent(other.name);
        }
    }
}
