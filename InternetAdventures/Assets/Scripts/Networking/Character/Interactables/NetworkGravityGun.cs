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

    public void SetFurthestDistanceToObject(float pValue)
    {
        _furthestDistanceToObject = pValue;
    }
    
    public List<ItemInformation> GetItems()
    {
        return _pickedUpObjects;
    }
    
    private void ActivateGun(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;
        
        ApplyCameraShake();
       
        networkInteractableManager.CmdActivateGravityGun(gravityRadius, range, interactableLayers, _furthestDistanceToObject);
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

    public void ResetObjectParent(ItemInformation pickedUpObject)
    {
        pickedUpObject.CurrentGameObject.transform.SetParent(pickedUpObject.Parent);
    }
    
    public void AddItemToPickedUpList(ItemInformation item, Transform parentTransform)
    {
        item.CurrentGameObject.transform.SetParent(parentTransform); 
        _pickedUpObjects.Add(item);
    }

    public float GetCurrentDistance()
    {
        return _currentAttractionDistance;
    }

    public float GetAttractionSpeed()
    {
        return attractionSpeed;
    }

    public float GetClosestDistance()
    {
        return closestAttractionDistance;
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