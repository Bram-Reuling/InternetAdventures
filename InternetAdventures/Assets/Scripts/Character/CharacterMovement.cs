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
    [SerializeField] private float jumpHeight;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float gravityMultiplier;
    private Quaternion newRotation;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Jump").performed += OnJump;
        _playerInput.actions.FindAction("Movement").performed += OnMoveDown;
        _playerInput.actions.FindAction("Movement").canceled += OnMoveUp;
    }
    
    private void Update()
    {
        _movement += _inputMovement;
        _movement *= 0.8f;
        if (!_characterController.isGrounded) _movement.y += Physics.gravity.y * gravityMultiplier;
        _characterController.Move(_movement * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.1f);
    }
    
    private void OnJump(InputAction.CallbackContext pObj)
    {
        if(_characterController.isGrounded) _movement.y = jumpHeight;
    }
    
    private void OnMoveDown(InputAction.CallbackContext pInputValue)
    {
        Vector2 movementVector = pInputValue.ReadValue<Vector2>();
        newRotation = Quaternion.LookRotation(new Vector3(movementVector.x,0, movementVector.y), Vector3.up);
        _inputMovement = new Vector3(movementVector.x, 0, movementVector.y) * movementSpeed;
    }
    
    private void OnMoveUp(InputAction.CallbackContext pInputValue)
    {
        Vector2 movementVector = pInputValue.ReadValue<Vector2>();
        _inputMovement = new Vector3(movementVector.x, 0, movementVector.y) * movementSpeed;
    }
}
