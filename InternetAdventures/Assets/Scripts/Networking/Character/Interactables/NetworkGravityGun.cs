using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using DG.Tweening;

public class NetworkGravityGun : Interactable
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
    private readonly RaycastHit[] _overlappedColliders = new RaycastHit[50];
    private float _furthestDistanceToObject;

    private void Start()
    {
        //Setup input
        playerInput.actions.FindAction("Interactable").started += ActivateGun;
        playerInput.actions.FindAction("Interactable").canceled += DeactivateGun;
        playerInput.actions.FindAction("Scroll").performed += ChangeAttractionDistance;
    }

    private void Update()
    {
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
        
        if(showDebugInfo) ShowDebugInformation();
    }

    private void ActivateGun(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;
        
        ApplyCameraShake();
        
        int foundColliders = Physics.SphereCastNonAlloc(transform.position, gravityRadius, transform.forward, _overlappedColliders, range, interactableLayers);
        if (foundColliders > 0)
        {
            for (int i = 0; i < foundColliders; i++)
            {
                GameObject intersectingGameObject = _overlappedColliders[i].collider.gameObject;
                Rigidbody currentRigidbody = intersectingGameObject.GetComponent<Rigidbody>();
                float currentDistance = (intersectingGameObject.transform.position - transform.parent.parent.position).magnitude;
                if (currentDistance > _furthestDistanceToObject) _furthestDistanceToObject = currentDistance;
                _pickedUpObjects.Add(new ItemInformation(intersectingGameObject, intersectingGameObject.transform.parent, currentRigidbody.constraints, currentDistance));
                intersectingGameObject.transform.SetParent(transform);
                currentRigidbody.useGravity = false;
                currentRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                CharacterMovement.weaponInUse = true;
            }
        }
    }

    private void ChangeAttractionDistance(InputAction.CallbackContext pCallback)
    {
        float yValue = pCallback.ReadValue<Vector2>().y * 0.01f;
        if (_currentAttractionDistance <= -_furthestDistanceToObject && yValue < 0)
            return;
        _currentAttractionDistance += yValue;
    }

    private void DeactivateGun(InputAction.CallbackContext pCallback)
    {
        //Sets parent to null again and clears list.
        foreach (var pickedObject in _pickedUpObjects)
        {
            pickedObject.CurrentGameObject.transform.SetParent(pickedObject.Parent);
            Rigidbody currentRigidbody = pickedObject.CurrentGameObject.GetComponent<Rigidbody>();
            currentRigidbody.constraints = pickedObject.RigidbodyConstraints;
            currentRigidbody.useGravity = true;
        }

        _pickedUpObjects.Clear();
        CharacterMovement.weaponInUse = false;
        _currentAttractionDistance = 0;
    }

    private void ShowDebugInformation()
    {
        Debug.DrawRay(transform.position, transform.forward * range, Color.magenta);
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