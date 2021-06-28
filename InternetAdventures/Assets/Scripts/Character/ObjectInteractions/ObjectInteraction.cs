using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    //Public
    [SerializeField] private float pushForce;
    
    //Private
    public LayerMask interactableLayers;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null)
        {
            if ((interactableLayers & (1 << hit.gameObject.layer)) > 0)
            {
                hit.rigidbody.AddForce((hit.transform.position - transform.position) * pushForce);
                Debug.Log("Successfully transferred force");
            }
        }
    }
}
