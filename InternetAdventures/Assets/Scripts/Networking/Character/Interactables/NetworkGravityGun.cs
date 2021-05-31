using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using DG.Tweening;

public class NetworkGravityGun : NetworkInteractable
{
    [Header("Interactable-specific attributes")]

    //Public
    [SerializeField] private float range;
    [SerializeField] private float gravityRadius;
    [SerializeField] private float attractionSpeed;
    [SerializeField] private float closestAttractionDistance;
    [SerializeField] private bool showDebugInfo;
    
    //Private
    private float _currentAttractionDistance;
    private readonly List<ItemInformation> _pickedUpObjects = new List<ItemInformation>();
    private float _furthestDistanceToObject;

    // Client
    private void Start()
    {
        //Setup input
        playerInput.actions.FindAction("Interactable").started += ActivateGun;
        playerInput.actions.FindAction("Interactable").canceled += DeactivateGun;
        playerInput.actions.FindAction("Scroll").performed += ChangeAttractionDistance;
    }

    private void Update()
    {
        // Server
        
        //Move objects towards player only if there's at least one.
        if (_pickedUpObjects.Count > 0)
        {
            foreach (var pickedObject in _pickedUpObjects)
            {
                GameObject currentGameObject = pickedObject.CurrentGameObject;
                Vector3 movementDirection = currentGameObject.transform.position - transform.parent.parent.position;
                float goalDistance = pickedObject.InitialDistance + _currentAttractionDistance;
                float deltaDistance = goalDistance - movementDirection.magnitude;
                if (movementDirection.magnitude + attractionSpeed * deltaDistance * Time.deltaTime < closestAttractionDistance) continue;
                if (Mathf.Abs(deltaDistance) > 0.1f)
                    currentGameObject.transform.Translate(attractionSpeed * deltaDistance * Time.deltaTime * movementDirection.normalized, Space.World);
            }
        }
        
        //if(showDebugInfo) ShowDebugInformation();
    }

    private void ActivateGun(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;
        
        ApplyCameraShake();
       
        networkInteractableManager.CmdActivateGravityGun(gravityRadius, range, interactableLayers, _furthestDistanceToObject, _pickedUpObjects);
    }

    private void ChangeAttractionDistance(InputAction.CallbackContext pCallback)
    {
        float yValue = pCallback.ReadValue<Vector2>().y * 0.01f;
        
        // Server
        networkInteractableManager.CmdChangeAttractionDistance(_currentAttractionDistance, _furthestDistanceToObject, yValue);
    }

    private void DeactivateGun(InputAction.CallbackContext pCallback)
    {
        networkInteractableManager.CmdDeactivateGravityGun(_pickedUpObjects);
        // Server
    }

    public void ChangeDistance(float pValue)
    {
        _currentAttractionDistance += pValue;
    }

    public void ResetDistance()
    {
        _currentAttractionDistance = 0;
    }

    public void ClearObjectList()
    {
        _pickedUpObjects.Clear();
    }
    
    public void AddItemToPickedUpList(ItemInformation item)
    {
        _pickedUpObjects.Add(item);
    }
}

public readonly struct ItemInformation
{
    public readonly GameObject CurrentGameObject;
    public readonly Transform Parent;
    public readonly RigidbodyConstraints RigidbodyConstraints;
    public readonly float InitialDistance;

    public ItemInformation(GameObject pGameObject, Transform pParent, RigidbodyConstraints pRigidbodyRigidbodyConstraints, float pInitialDistance)
    {
        CurrentGameObject = pGameObject;
        Parent = pParent;
        RigidbodyConstraints = pRigidbodyRigidbodyConstraints;
        InitialDistance = pInitialDistance;
    }
}