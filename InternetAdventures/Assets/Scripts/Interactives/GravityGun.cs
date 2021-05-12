using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GravityGun : Interactable
{
    private PlayerInput _playerInput;
    [SerializeField] private float range;
    [SerializeField] private float gravityRadius;
    [SerializeField] private float attractionSpeed;
    private float currentAttractionDistance;
    [SerializeField] private float AttractionDistance;
    [SerializeField] private bool showDebugInfo;
    private Dictionary<GameObject, Transform> _pickedUpObjects = new Dictionary<GameObject, Transform>();
    [SerializeField] private LayerMask interactableLayers;

    private void Start()
    {
        //Setup input
        _playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").started += ShootGun;
        _playerInput.actions.FindAction("Interactable").canceled += ReleaseObjects;
        _playerInput.actions.FindAction("Scroll").performed += ChangeAttractionDistance;
    }

    private void Update()
    {
        //Move objects towards player only if there's at least one.
        if (_pickedUpObjects.Count > 0)
        {
            foreach (var pickedObject in _pickedUpObjects.Keys)
            {
                Vector3 movementDirection = transform.parent.position - pickedObject.transform.position;
                float distanceDelta = movementDirection.magnitude - currentAttractionDistance;
                if (Mathf.Abs(distanceDelta) > 0.1f)
                {
                    if (distanceDelta > 0 && movementDirection.magnitude <= 4.0f) continue;
                    pickedObject.transform.Translate( attractionSpeed * distanceDelta * Time.deltaTime * movementDirection, Space.World);
                    pickedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    pickedObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }
        }
        if(showDebugInfo) ShowDebugInformation();
    }

    private void ShootGun(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;
        currentAttractionDistance = AttractionDistance;
        RaycastHit[] overlapColliders = Physics.SphereCastAll(transform.position, gravityRadius, transform.forward, range, interactableLayers);
        if (overlapColliders.Length > 0)
        {
            foreach (var intersectingObject in overlapColliders)
            {
                GameObject intersectingGameObject = intersectingObject.collider.gameObject;
                _pickedUpObjects.Add(intersectingGameObject, intersectingObject.collider.transform.parent);
                intersectingGameObject.transform.SetParent(transform.parent);
                intersectingObject.transform.GetComponent<Rigidbody>().useGravity = false;
                intersectingObject.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                CharacterMovement.weaponInUse = true;
            }
        }
    }

    private void ChangeAttractionDistance(InputAction.CallbackContext pCallback)
    {
        currentAttractionDistance += pCallback.ReadValue<Vector2>().y * 0.01f;
    }

    private void ReleaseObjects(InputAction.CallbackContext pCallback)
    {
        //Sets parent to null again and clears list.
        foreach (var pickedObject in _pickedUpObjects.Keys)
        {
            pickedObject.transform.SetParent(null);
            pickedObject.transform.GetComponent<Rigidbody>().useGravity = true;
            pickedObject.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            pickedObject.transform.parent = _pickedUpObjects[pickedObject];
        }

        _pickedUpObjects.Clear();
        CharacterMovement.weaponInUse = false;
    }

    private void ShowDebugInformation()
    {
        Debug.DrawRay(transform.position, transform.forward * range, Color.magenta);
    }
}