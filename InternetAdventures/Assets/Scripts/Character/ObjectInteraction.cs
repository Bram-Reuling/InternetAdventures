using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{

    [SerializeField] private float pushForce;
    public LayerMask interactableLayers;

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.rigidbody != null)
            hit.rigidbody.AddForce((hit.transform.position - transform.position) * pushForce);
    }
}
