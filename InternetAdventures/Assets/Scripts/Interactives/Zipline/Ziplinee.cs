using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ziplinee : MonoBehaviour
{
    private CharacterController _characterController;
    private CharacterMovement _characterMovement;
    private PlayerInput _playerInput;
    private Vector3 _ziplineVector;
    private Zipline _currentZipline;
    [SerializeField] private float minDistanceToZipline;
    
    private void Start()
    {
        //Get components
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _characterMovement = GetComponent<CharacterMovement>();
        //Setup input
        _playerInput.actions.FindAction("Jump").performed += AttachToZipline;
        _playerInput.actions.FindAction("Jump").canceled += DetachFromZipline;
    }

    private void Update()
    {
        if (_ziplineVector != Vector3.zero)
        {
            _characterController.Move(Time.deltaTime * 10 * _ziplineVector);
            if(!_currentZipline.ZiplineUsable(transform.position)) DetachFromZipline();
        }
    }

    private void AttachToZipline(InputAction.CallbackContext pCallback)
    {
        if (!_characterController.isGrounded)
        {
            GameObject[] ziplines = GameObject.FindGameObjectsWithTag("Zipline");
            foreach (var zipline in ziplines)
            {
                Zipline currentZipline = zipline.GetComponent<Zipline>();
                if (!currentZipline.ZiplineUsable(transform.position)) continue;
                Vector3 PlayerToLineVec = currentZipline.GetShortestVectorToLine(transform.position);
                if (PlayerToLineVec.magnitude <= minDistanceToZipline)
                {
                    Debug.Log("Attach to zipline!");
                    Debug.Log("Distance was " + PlayerToLineVec.magnitude);
                    
                    
                    _ziplineVector = currentZipline.GetNormalizedDirectionVector();
                    _ziplineVector *= Mathf.Sign(Vector3.Dot(transform.forward, _ziplineVector));
                    
                    _currentZipline = currentZipline;
                    CharacterMovement.onZipline = true;
                }
            }
        }
    }

    private void DetachFromZipline(InputAction.CallbackContext pCallback)
    {
        if (_ziplineVector == Vector3.zero) return;
        _ziplineVector = Vector3.zero;
        CharacterMovement.onZipline = false;
        _characterMovement.AddJumpForce();
    }
    private void DetachFromZipline()
    {
        if (_ziplineVector == Vector3.zero) return;
        _ziplineVector = Vector3.zero;
        CharacterMovement.onZipline = false;
        _characterMovement.AddJumpForce();
    }
}
