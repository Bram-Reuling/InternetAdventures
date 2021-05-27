using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkBanHammer : NetworkInteractable
{
    [Header("Interactable-specific attributes")]
    
    //Public
    [SerializeField] private bool enableScaleEffectOnObjects;
    
    //Private
    private readonly List<GameObject> _gameObjectsInTrigger = new List<GameObject>();
    private Vector3 _initialScale;

    private void Start()
    {
        //Setup input
        playerInput.actions.FindAction("Interactable").performed += SlamHammer;
        _initialScale = transform.localScale;
    }

    private void SlamHammer(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;
        ApplyCameraShake();
        
        networkInteractableManager.CmdSlamHammer(_gameObjectsInTrigger, enableScaleEffectOnObjects, _initialScale);
    }
    
    //Info: The purpose of this method is to cache all gameObjects that are currently in my trigger, so I can use
    //that list when the hammer is being slammed.
    private void OnTriggerEnter(Collider other)
    {
        if (!_gameObjectsInTrigger.Contains(other.gameObject))
        {
            //Only add game object when its in the interactable layers.
            if((interactableLayers.value & (1 << other.gameObject.layer)) > 0)
                _gameObjectsInTrigger.Add(other.gameObject);
        }
    }    
    
    //Info: The purpose here is to remove the previously added game objects from the list.
    private void OnTriggerExit(Collider other)
    {
        if(_gameObjectsInTrigger.Contains(other.gameObject)) 
            _gameObjectsInTrigger.Remove(other.gameObject);
    }
}
