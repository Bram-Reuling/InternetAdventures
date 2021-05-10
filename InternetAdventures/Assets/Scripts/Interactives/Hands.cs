using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hands : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private float grabDistance;
    private Rigidbody _grabbedObject;
    
    private void Start()
    {
        //Setup input
        _playerInput = transform.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").started += GrabObjectInFront;
        _playerInput.actions.FindAction("Interactable").canceled += ReleaseObject;
    }

    private void FixedUpdate()
    {
        if (_grabbedObject != null)
        {
            Vector3 directionVector = transform.parent.position - _grabbedObject.position;
            if (directionVector.magnitude > 3.0f)
            {
                Vector3 forceToBeApplied = 20.0f * directionVector.normalized;
                _grabbedObject.AddForce(forceToBeApplied, ForceMode.Force);
            }
            else _grabbedObject.velocity *= 0.95f;
        }
    }

    private void GrabObjectInFront(InputAction.CallbackContext pCallback)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, grabDistance))
        {
            if (raycastHit.transform.TryGetComponent(out _grabbedObject))
            {
                Debug.Log("Object grabbed");
            }
        }
    }

    private void ReleaseObject(InputAction.CallbackContext pCallback)
    {
        _grabbedObject = null;
    }
}
