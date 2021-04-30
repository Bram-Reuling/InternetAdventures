using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController _characterController;
    private PlayerInput _playerInput;
    private Vector3 _movement;
    private Vector3 _inputMovement;
    private Quaternion newRotation;
    
    //Public attributes
    [SerializeField] private float jumpHeight;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float deceleration;

    private void Start()
    {
        //Get components
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        //Setup input
        _playerInput.actions.FindAction("Jump").performed += OnJump;
        _playerInput.actions.FindAction("Movement").performed += OnMoveDown;
        _playerInput.actions.FindAction("Movement").canceled += OnMoveUp;
    }
    
    private void Update()
    {
        //Decelerate
        _movement *= deceleration;
        //Add current input movement to actual movement
        _movement += _inputMovement;
        //Apply jump force only when character is grounded.
        if (!_characterController.isGrounded) _movement.y += Physics.gravity.y * gravityMultiplier;
        //Move character controller
        _characterController.Move(_movement * Time.deltaTime);
        //Add rotation to the character controller based on the current movement speed, so the character
        //does not rotate when not walking. The threshold is there to prevent false movement since movement has a magnitude even when
        //standing still.
        if(_movement.magnitude > 5.0f) transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, _movement.magnitude * 0.5f);
    }
    
    private void OnJump(InputAction.CallbackContext pObj)
    {
        //Apply jump force
        if(_characterController.isGrounded) _movement.y = jumpHeight;
    }
    
    private void OnMoveDown(InputAction.CallbackContext pInputValue)
    {
        //Read in new vector
        Vector2 movementVector = pInputValue.ReadValue<Vector2>();
        //Create new rotation from the given input
        newRotation = Quaternion.LookRotation(new Vector3(movementVector.x,0, movementVector.y), Vector3.up);
        //Assign movement direction multiplied with the movement speed
        _inputMovement = new Vector3(movementVector.x, 0, movementVector.y) * movementSpeed;
    }
    
    private void OnMoveUp(InputAction.CallbackContext pInputValue)
    {
        //This is just to assign a zero vector to the movement vector again,
        //since the new input system does not continuously call 'performed' when button is held down.
        Vector2 movementVector = pInputValue.ReadValue<Vector2>();
        _inputMovement = new Vector3(movementVector.x, 0, movementVector.y) * movementSpeed;
    }
}