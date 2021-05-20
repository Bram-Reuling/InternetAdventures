using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class BanHammer : Interactable
{
    //Public
    [SerializeField] private bool enableScaleEffectOnObjects;
    
    //Private
    private PlayerInput _playerInput;
    private readonly List<GameObject> _gameObjectsInTrigger = new List<GameObject>();

    private void Start()
    {
        _playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").performed += SlamHammer;
    }

    private void SlamHammer(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;
        if (enableCameraShake)
        {
            Camera.main.DOShakePosition(cameraShakeDuration, cameraShakeStrength);
            Camera.main.DOShakeRotation(cameraShakeDuration, cameraShakeStrength);   
        }

        foreach (var gameObjectInReach in _gameObjectsInTrigger)
        {
            //IMPORTANT
            //Todo: This line is subject to change depending on what the designers want
            
            if (!enableScaleEffectOnObjects)
            {
                //Info: Scale object down.
                gameObjectInReach.transform.localScale -= new Vector3(0, gameObjectInReach.transform.localScale.y - 0.2f, 0);
                //Info: Send ray to ground and place object there.
                if (Physics.Raycast(gameObjectInReach.transform.position, Vector3.down, out var raycastHit,
                    float.PositiveInfinity))
                {
                    gameObjectInReach.transform.position = raycastHit.point + new Vector3(0,gameObjectInReach.transform.lossyScale.y / 2,0);
                }   
            }
            else gameObjectInReach.transform.DOShakeScale(1, 2f);
            
            //Add impulse upwards if there's a rigidbody.
            Rigidbody rigidbody = gameObjectInReach.GetComponent<Rigidbody>();
            if (rigidbody != null)
                rigidbody.AddForce(Vector3.up * 5.0f, ForceMode.Impulse);
        }
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
