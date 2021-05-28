using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkShockwaveGun : NetworkInteractable
{
    [Header("Interactable-specific attributes")]
    
    //Public
    [SerializeField] private float range;
    [SerializeField] private float shockwaveRadius;
    [SerializeField] private float shockwaveStrength;
    [SerializeField] private float possibleHitRadius;
    [SerializeField] private bool showDebugInfo;

    private void Start()
    {
        //Setup input
        playerInput.actions.FindAction("Interactable").performed += ShootGun;
    }

    private void Update()
    {
        if (showDebugInfo) DrawDebug();
    }

    private void ShootGun(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;

        networkInteractableManager.CmdShootShockwaveGun(range,shockwaveRadius, shockwaveStrength, possibleHitRadius, interactableLayers);
        
        ApplyCameraShake();
    }

    private void DrawDebug()
    {
        Debug.DrawRay(transform.position, transform.forward * range, Color.magenta);
    }
}