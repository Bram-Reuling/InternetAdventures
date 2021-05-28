using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]

public class NetworkHands : NetworkInteractable
{
    [Header("Interactable-specific attributes")]
    
    //Public
    //[SerializeField] private NetworkHandMode handMode;
    
    //Private
    [SerializeField] private GameObject _grabbedObject;
    private Transform _initialParent;
    private readonly List<GameObject> _gameObjectsInTrigger = new List<GameObject>();
    private NetworkCharacterMovement _characterMovement;
    
    private void Start()
    {
        //Setup input
        playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Interactable").started += GrabObjectInFront;
        playerInput.actions.FindAction("Interactable").canceled += ReleaseObject;

        //Get components
        _characterMovement = transform.parent.parent.GetComponent<NetworkCharacterMovement>();
    }

    public void SetGrabbedObject(GameObject grabbedObject)
    {
        _grabbedObject = grabbedObject;
    }

    public void SetInitialParent(Transform initialParent)
    {
        _initialParent = initialParent;
    }
    
    private void GrabObjectInFront(InputAction.CallbackContext pCallback)
    {
        if(_gameObjectsInTrigger.Count == 0 || !gameObject.activeSelf) return;
        
        // Code for the server
        float shortestDistanceGameObject = float.PositiveInfinity;
        foreach (var currentGameObject in _gameObjectsInTrigger)
        {
            if ((currentGameObject.transform.position - transform.parent.transform.position).magnitude < shortestDistanceGameObject)
            {
                _grabbedObject = currentGameObject;
            }
        }

        if (_grabbedObject == null) return;

        // Server and client
        _initialParent = _grabbedObject.transform.parent;
        _grabbedObject.transform.parent = transform;
        _grabbedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void ReleaseObject(InputAction.CallbackContext pCallback)
    {
        if (_grabbedObject == null) return;
        
        // Server and client
        _grabbedObject.transform.parent = _initialParent;
        Rigidbody objectRigidbody = _grabbedObject.GetComponent<Rigidbody>();
        objectRigidbody.constraints = RigidbodyConstraints.None;
        objectRigidbody.AddForce(_characterMovement.GetVelocity() * 50);
        
        _grabbedObject = null;   
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
    
}

public enum NetworkHandMode{ Drag, Pickup}
