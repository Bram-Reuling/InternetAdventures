using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]

public class NetworkHands : NetworkInteractable
{
    [Header("Interactable-specific attributes")]

    //Private
    [SerializeField] private GameObject _grabbedObject = null;
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

    #region Getters and Setters

    public void SetGrabbedObject(GameObject grabbedObject)
    {
        _grabbedObject = grabbedObject;
    }

    public void SetInitialParent(Transform initialParent)
    {
        _initialParent = initialParent;
    }

    public Transform GetInitialParent()
    {
        return _initialParent;
    }

    public void SetGrabbedObjectParent(Transform pTransform)
    {
        if (_grabbedObject == null) return;
        _grabbedObject.transform.parent = pTransform;
    }

    public void SetGrabbedObjectConstraints(RigidbodyConstraints constraints)
    {
        if (_grabbedObject == null) return;
        _grabbedObject.GetComponent<Rigidbody>().constraints = constraints;
    }

    #endregion

    public void AddForceToGrabbedRigidbody(float pValue)
    {
        if (_grabbedObject == null) return;
        _grabbedObject.GetComponent<Rigidbody>().AddForce(_characterMovement.GetVelocity() * pValue);
    }
    
    private void GrabObjectInFront(InputAction.CallbackContext pCallback)
    {
        if(_gameObjectsInTrigger.Count == 0 || !gameObject.activeSelf) return;
        
        networkInteractableManager.CmdGrabObjectInFront(_gameObjectsInTrigger, _grabbedObject);
    }

    private void ReleaseObject(InputAction.CallbackContext pCallback)
    {
        if (_grabbedObject == null) return;
        
        networkInteractableManager.CmdReleaseObject(_initialParent);
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
