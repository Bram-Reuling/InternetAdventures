using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController _characterController;
    
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
