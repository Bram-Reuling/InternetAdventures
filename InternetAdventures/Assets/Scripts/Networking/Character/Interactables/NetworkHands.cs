using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]

public class NetworkHands : Interactable
{
    [Header("Interactable-specific attributes")]
    
    //Public
    [SerializeField] private HandMode handMode;
    
    //Private
    private GameObject _grabbedObject;
    private Transform _initialParent;
    private readonly List<GameObject> _gameObjectsInTrigger = new List<GameObject>();
    private CharacterMovement _characterMovement;
    
    private void Start()
    {
        //Setup input
        playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Interactable").started += GrabObjectInFront;
        playerInput.actions.FindAction("Interactable").canceled += ReleaseObject;

        //Get components
        _characterMovement = transform.parent.parent.GetComponent<CharacterMovement>();
    }

    private void FixedUpdate()
    {
        if (_grabbedObject != null)
        {
            if (handMode == HandMode.Drag)
            {
                Vector3 directionVector = transform.parent.position - _grabbedObject.transform.position;
                if (directionVector.magnitude > 3.0f)
                {
                    Vector3 forceToBeApplied = 30.0f * directionVector.normalized;
                    _grabbedObject.GetComponent<Rigidbody>().AddForce(forceToBeApplied, ForceMode.Force);
                }
                else _grabbedObject.GetComponent<Rigidbody>().velocity *= 0.95f;
            }
        }
    }

    private void GrabObjectInFront(InputAction.CallbackContext pCallback)
    {
        if(_gameObjectsInTrigger.Count == 0 || !gameObject.activeSelf) return;
        float shortestDistanceGameObject = float.PositiveInfinity;
        foreach (var currentGameObject in _gameObjectsInTrigger)
        {
            if ((currentGameObject.transform.position - transform.parent.transform.position).magnitude < shortestDistanceGameObject)
            {
                _grabbedObject = currentGameObject;
            }
        }

        if (_grabbedObject == null) return;
        if (handMode == HandMode.Pickup)
        {
            _initialParent = _grabbedObject.transform.parent;
            _grabbedObject.transform.parent = transform;
            _grabbedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_gameObjectsInTrigger.Contains(other.gameObject))
        {
            if((interactableLayers.value & (1 << other.gameObject.layer)) > 0)
                _gameObjectsInTrigger.Add(other.gameObject);
        }
    }    
    
    private void OnTriggerExit(Collider other)
    {
        if(_gameObjectsInTrigger.Contains(other.gameObject)) 
            _gameObjectsInTrigger.Remove(other.gameObject);
    }

    private void ReleaseObject(InputAction.CallbackContext pCallback)
    {
        if (_grabbedObject == null) return;
        _grabbedObject.transform.parent = _initialParent;
        if (handMode == HandMode.Pickup)
        {
            Rigidbody objectRigidbody = _grabbedObject.GetComponent<Rigidbody>();
            objectRigidbody.constraints = RigidbodyConstraints.None;
            objectRigidbody.AddForce(_characterMovement.GetVelocity() * 50);
        }
        _grabbedObject = null;   
    }
}

public enum NetworkHandMode{ Drag, Pickup}
