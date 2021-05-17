using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Walk : CharacterState
{
    [SerializeField] private float deceleration;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationOnMovementMultiplier;
    private CharacterFSMInformation _characterFsmInformation;
    private Vector3 _inputMovement;
    private Vector3 _velocity;
    private Quaternion _newRotation;

    public override void Initialize(CharacterFSM pCharacterFSM)
    {
        base.Initialize(pCharacterFSM);
        PlayerInput characterInput = pCharacterFSM.CharacterFsmInformation.PlayerInput;
        characterInput.actions.FindAction("Movement").performed += OnMoveDown;
        characterInput.actions.FindAction("Movement").canceled += OnMoveUp;
        _characterFsmInformation = pCharacterFSM.CharacterFsmInformation;
    }

    public override void EnterState()
    {
        base.EnterState();
        _velocity = _characterFsmInformation.Velocity;
    }

    public override void Update()
    {
        base.Update();
        
        //Decelerate
        if (_inputMovement.magnitude <= 0.1f)
        {
            _velocity.x *= deceleration;
            _velocity.z *= deceleration;
        }
        
        //Add current input movement to actual movement
        _velocity += _inputMovement;
        
        Vector3 XZMovement = new Vector3(_velocity.x, 0, _velocity.z);
        if(XZMovement.magnitude > movementSpeed)
        {
            XZMovement.Normalize();
            XZMovement *= movementSpeed;
            _velocity.x = XZMovement.x;
            _velocity.z = XZMovement.z;
        }
        
        _characterFsmInformation.CharacterController.Move(_velocity * Time.deltaTime);
        //Add rotation to the character controller based on the current movement speed, so the character
        //does not rotate when not walking. The threshold is there to prevent false movement since movement has a magnitude even when
        //standing still.
        if(XZMovement.magnitude > 2.0f) transform.rotation = Quaternion.RotateTowards(transform.rotation, _newRotation, 
            XZMovement.magnitude * rotationOnMovementMultiplier);

        //Exit condition
        if (_velocity.magnitude < 0.01f)
            _characterFSM.ChangeState<Idle>();
    }

    public override void ExitState()
    {
        base.ExitState();
        _characterFsmInformation.Velocity = _velocity;
    }

    private void OnMoveDown(InputAction.CallbackContext pInputValue)
    {
        //Read in new vector
        Vector2 movementVector = pInputValue.ReadValue<Vector2>();
        //Create new rotation from the given input
        _newRotation = Quaternion.LookRotation(new Vector3(movementVector.x,0, movementVector.y), Vector3.up);
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
