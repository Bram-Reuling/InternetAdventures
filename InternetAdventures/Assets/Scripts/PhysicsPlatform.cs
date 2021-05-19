using System;
using UnityEngine;

public class PhysicsPlatform : MonoBehaviour
{
    [SerializeField] private PhysicsPlatformHandler _physicsPlatformHandler;

    private void OnCollisionEnter(Collision other)
    {
        _physicsPlatformHandler.OnActuation(gameObject, other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        
    }
}
