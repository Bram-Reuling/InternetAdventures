using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Animations;

public class GravityGun : Interactable
{
    private PlayerInput _playerInput;
    [SerializeField] private float range;
    [SerializeField] private float gravityRadius;
    [SerializeField] private float attractionSpeed;
    private List<GameObject> _pickedUpObjects = new List<GameObject>();
    
    private void Start()
    {
        _playerInput = transform.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").started += ShootGun;
        _playerInput.actions.FindAction("Interactable").canceled += ReleaseObjects;
    }

    private void Update()
    {
        if (_pickedUpObjects.Count > 0)
        {
            foreach (var pickedObject in _pickedUpObjects)
            {
                Vector3 movementDirection = transform.parent.position - pickedObject.transform.position;
                //Quaternion necessaryRotation =
                    //Quaternion.FromToRotation(movementDirection, transform.parent.rotation * movementDirection);
                //pickedObject.transform.position = transform.parent.position + necessaryRotation * movementDirection;
                
                if(movementDirection.magnitude > 3.0f)
                    pickedObject.transform.Translate( attractionSpeed * Time.deltaTime * movementDirection, Space.World);
            }
        }
    }

    private void ShootGun(InputAction.CallbackContext pCallback)
    {
        if (Physics.Raycast(transform.position, transform.forward, out var raycastHit, range))
        {
            Vector3 hitPosition = raycastHit.point;
            Collider[] collidersInRange = Physics.OverlapSphere(hitPosition, gravityRadius);
            foreach (var objectInCollider in collidersInRange)
            {
                if (objectInCollider.TryGetComponent(typeof(Rigidbody), out var objectRigidbody))
                {
                    Destroy(objectRigidbody);
                    _pickedUpObjects.Add(objectInCollider.gameObject);
                }
            }
        }
    }

    private void ReleaseObjects(InputAction.CallbackContext pCallback)
    {
        foreach (var pickedObject in _pickedUpObjects)
        {
            pickedObject.AddComponent<Rigidbody>();
        }

        _pickedUpObjects.Clear();
    }
}