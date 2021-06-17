using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class NetworkShockwaveGun : NetworkInteractable
{
    [Header("Interactable-specific attributes")]
    
    //Public
    [SerializeField] private float range;
    [SerializeField] private float shockwaveRadius;
    [SerializeField] private float shockwaveStrength;
    [SerializeField] private float possibleHitRadius;
    [SerializeField] private bool showDebugInfo;
    [SerializeField] private VisualEffect effect;

    private void Start()
    {
        //Setup input
        playerInput.actions.FindAction("Interactable").performed += ShootGun;
        effect.playRate = 3;
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

    public void PlayEffect()
    {
        effect.Play();
    }
    
    private void DrawDebug()
    {
        Debug.DrawRay(transform.position, transform.forward * range, Color.magenta);
    }
}