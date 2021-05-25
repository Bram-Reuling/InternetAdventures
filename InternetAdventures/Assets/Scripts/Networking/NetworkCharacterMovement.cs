﻿using GameCamera;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Networking
{
    public class NetworkCharacterMovement : NetworkBehaviour
    {
        #region Variables

        //Public
        [HideInInspector] public bool UserInputAllowed;

        //Private - Inspector
        [SerializeField] private float jumpHeight;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float gravityMultiplier;
        [SerializeField] private float rotationOnMovementMultiplier;
        [SerializeField] private float deceleration;

        //Private
        private Vector3 _velocity;
        private Vector3 _inputMovement;
        private Vector3 _externalMovement = Vector3.zero;
        private Quaternion _newRotation;
        private GameObject _currentlyCollidingGameObject;
        private bool _collideEveryFrame;

        public static bool weaponInUse;

        //Other components
        private CharacterController _characterController;
        private PlayerInput _playerInput;

        #endregion

        #region Global Functions
        
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();

            UserInputAllowed = true;
            
            ClientStart();
        }
        
        public Vector3 GetVelocity()
        {
            return _velocity;
        }

        #endregion
        
        #region Server Functions

        [ServerCallback]
        private void Update()
        {
            Decelerate();
            //Add current input movement to actual movement
            _velocity += _inputMovement;
            Vector3 xzMovement = ConstrainXZMovement();
            //Apply jump force only when character is grounded.
            if (!_characterController.isGrounded)
            {
                _velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
                OnCollisionLeave();
                _currentlyCollidingGameObject = null;
            }

            //Move character controller
            if (UserInputAllowed) _characterController.Move((_velocity + _externalMovement) * Time.deltaTime);
            //Add rotation to the character controller based on the current movement speed, so the character
            //does not rotate when not walking. The threshold is there to prevent false movement since movement has a magnitude even when
            //standing still.
            if (xzMovement.magnitude > 2.0f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _newRotation,
                    xzMovement.magnitude * rotationOnMovementMultiplier);
        }
        
        [ServerCallback]
        private void Decelerate()
        {
            if (!(_inputMovement.magnitude <= 0.1f)) return;
            _velocity.x *= deceleration;
            _velocity.z *= deceleration;
        }

        [ServerCallback]
        private Vector3 ConstrainXZMovement()
        {
            Vector3 xzMovement = new Vector3(_velocity.x, 0, _velocity.z);
            if (xzMovement.magnitude > movementSpeed)
            {
                xzMovement.Normalize();
                xzMovement *= movementSpeed;
                _velocity.x = xzMovement.x;
                _velocity.z = xzMovement.z;
            }

            return xzMovement;
        }
        
        [ServerCallback]
        private void OnCollisionLeave()
        {
            //Note: This case is only true, when the character jumps.
            if (_currentlyCollidingGameObject == null) return;
            switch (_currentlyCollidingGameObject.transform.tag)
            {
                case "Platform":
                    _collideEveryFrame = false;
                    _externalMovement = Vector3.zero;
                    break;
                case "PhysicsPlatform":
                    _currentlyCollidingGameObject.transform.parent.GetComponent<PhysicsPlatformHandler>()
                        .StopActuation();
                    break;
            }
        }
        
        [ServerCallback]
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //Note: Only recognize collision once, 'OnControllerColliderHit' is being called every frame.
            if (_currentlyCollidingGameObject == hit.gameObject && !_collideEveryFrame) return;
            OnCollisionLeave();
            _currentlyCollidingGameObject = hit.gameObject;
            switch (_currentlyCollidingGameObject.transform.tag)
            {
                case "Platform":
                    //TODO: Cache this thing.
                    _externalMovement = _currentlyCollidingGameObject.GetComponent<MovingPlatform>()
                        .CurrentMovementVector;
                    _collideEveryFrame = true;
                    break;
                case "PhysicsPlatform":
                    _currentlyCollidingGameObject.transform.parent
                        .GetComponent<PhysicsPlatformHandler>().OnActuation(_currentlyCollidingGameObject, gameObject);
                    break;
            }
        }
        
        [Command]
        private void CmdOnMoveDown(Vector2 movementVector)
        {
            Debug.Log("CmdOnMoveDown");
            //Create new rotation from the given input
            _newRotation = Quaternion.LookRotation(new Vector3(movementVector.x, 0, movementVector.y), Vector3.up);
            //Assign movement direction multiplied with the movement speed
            _inputMovement = new Vector3(movementVector.x, 0, movementVector.y) * movementSpeed;
        }

        [Command]
        private void CmdOnMoveUp(Vector2 movementVector)
        {
            Debug.Log("CmdOnMoveUp");
            _inputMovement = new Vector3(movementVector.x, 0, movementVector.y) * movementSpeed;
        }

        [Command]
        private void CmdOnJump()
        {
            Debug.Log("CmdOnJump");
            //Apply jump force
            if (_characterController.isGrounded) _velocity.y = jumpHeight;
        }

        #endregion

        #region Client Functions

        public override void OnStartAuthority()
        {
            CameraRig cameraRig = FindObjectOfType<CameraRig>();

            if (cameraRig)
            {
                cameraRig.Target = gameObject;
            }
        }

        [ClientCallback]
        private void OnMoveDown(InputAction.CallbackContext pInputValue)
        {
            Debug.Log("Requesting CmdOnMoveDown");
            //Read in new vector
            Vector2 movementVector = pInputValue.ReadValue<Vector2>();
            CmdOnMoveDown(movementVector);
        }

        [ClientCallback]
        private void OnMoveUp(InputAction.CallbackContext pInputValue)
        {
            Debug.Log("Requesting CmdOnMoveUp");
            //This is just to assign a zero vector to the movement vector again,
            //since the new input system does not continuously call 'performed' when button is held down.
            Vector2 movementVector = pInputValue.ReadValue<Vector2>();
            CmdOnMoveUp(movementVector);
        }
        
        [ClientCallback]
        private void OnJump(InputAction.CallbackContext pObj)
        {
            Debug.Log("Requesting CmdOnJump");
            CmdOnJump();
        }
        
        [ClientCallback]
        private void ClientStart()
        {
            if (!isLocalPlayer) return;
            
            //Get components
            _playerInput = GetComponent<PlayerInput>();

            //Setup input
            //Movement
            _playerInput.actions.FindAction("Movement").performed += OnMoveDown;
            _playerInput.actions.FindAction("Movement").canceled += OnMoveUp;
            //Jump
            _playerInput.actions.FindAction("Jump").performed += OnJump;
        }
        
        #endregion
    }
}