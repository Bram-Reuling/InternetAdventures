using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GravityGun : Interactable
{
    private PlayerInput _playerInput;
    [SerializeField] private float range;
    [SerializeField] private float gravityRadius;
    [SerializeField] private float attractionSpeed;
    [SerializeField] private float minAttractionDistance;
    [SerializeField] private bool showDebugInfo;
    private List<GameObject> _pickedUpObjects = new List<GameObject>();

    private void Start()
    {
        //Setup input
        _playerInput = transform.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").started += ShootGun;
        _playerInput.actions.FindAction("Interactable").canceled += ReleaseObjects;
        _playerInput.actions.FindAction("Scroll").performed += ChangeAttractionDistance;
    }

    private void Update()
    {
        //Move objects towards player only if there's at least one.
        if (_pickedUpObjects.Count > 0)
        {
            foreach (var pickedObject in _pickedUpObjects)
            {
                Vector3 movementDirection = transform.parent.position - pickedObject.transform.position;
                float distanceDelta = movementDirection.magnitude - minAttractionDistance;
                if (Mathf.Abs(distanceDelta) > 0.5f)
                {
                    pickedObject.transform.Translate( attractionSpeed * Time.deltaTime * movementDirection * distanceDelta, Space.World);
                }
            }
        }
        if(showDebugInfo) ShowDebugInformation();
    }

    private void ShootGun(InputAction.CallbackContext pCallback)
    {
        LayerMask layerMask = LayerMask.GetMask("Enemy");
        RaycastHit[] test = Physics.SphereCastAll(transform.position, gravityRadius, transform.forward, range, layerMask);
        if (test.Length > 0)
        {
            foreach (var intersectingObject in test)
            {
                GameObject intersectingGameObject = intersectingObject.collider.gameObject;
                _pickedUpObjects.Add(intersectingGameObject);
                intersectingGameObject.transform.SetParent(transform.parent);
                intersectingObject.transform.GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }

    private void ChangeAttractionDistance(InputAction.CallbackContext pCallback)
    {
        minAttractionDistance += pCallback.ReadValue<Vector2>().y * 0.01f;
        Debug.Log(minAttractionDistance);
    }

    private void ReleaseObjects(InputAction.CallbackContext pCallback)
    {
        //Sets parent to null again and clears list.
        foreach (var pickedObject in _pickedUpObjects)
        {
            pickedObject.transform.SetParent(null);
            pickedObject.transform.GetComponent<Rigidbody>().useGravity = true;
        }

        _pickedUpObjects.Clear();
    }

    private void ShowDebugInformation()
    {
        Debug.DrawRay(transform.position, transform.forward * range, Color.magenta);
    }
}