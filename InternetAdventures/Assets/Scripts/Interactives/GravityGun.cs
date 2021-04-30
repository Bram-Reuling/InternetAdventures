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
    [SerializeField] private float minAttractionDistance;
    private List<GameObject> _pickedUpObjects = new List<GameObject>();
    
    private void Start()
    {
        //Setup input
        _playerInput = transform.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").started += ShootGun;
        _playerInput.actions.FindAction("Interactable").canceled += ReleaseObjects;
    }

    private void Update()
    {
        //Move objects towards player only if there's at least one.
        if (_pickedUpObjects.Count > 0)
        {
            foreach (var pickedObject in _pickedUpObjects)
            {
                Vector3 movementDirection = transform.parent.position - pickedObject.transform.position;
                if(movementDirection.magnitude > minAttractionDistance)
                    pickedObject.transform.Translate( attractionSpeed * Time.deltaTime * movementDirection, Space.World);
            }
        }
    }

    private void ShootGun(InputAction.CallbackContext pCallback)
    {
        //Shoots raycast, creates sphere collider, adds collision references to list and set the character as their parent.
        if (Physics.Raycast(transform.position, transform.forward, out var raycastHit, range))
        {
            Vector3 hitPosition = raycastHit.point;
            Collider[] collidersInRange = Physics.OverlapSphere(hitPosition, gravityRadius);
            foreach (var objectInCollider in collidersInRange)
            {
                if (objectInCollider.TryGetComponent(typeof(Rigidbody), out var objectRigidbody))
                {
                    _pickedUpObjects.Add(objectInCollider.gameObject);
                    objectInCollider.transform.SetParent(transform.parent);
                }
            }
        }
    }

    private void ReleaseObjects(InputAction.CallbackContext pCallback)
    {
        //Sets parent to null again and clears list.
        foreach (var pickedObject in _pickedUpObjects)
        {
            pickedObject.transform.SetParent(null);
        }

        _pickedUpObjects.Clear();
    }
}