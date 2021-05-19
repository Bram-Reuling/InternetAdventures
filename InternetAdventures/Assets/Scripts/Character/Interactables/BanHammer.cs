using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class BanHammer : Interactable
{
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private bool shakeEffect;
    private PlayerInput _playerInput;
    private List<GameObject> _gameObjectsInTrigger = new List<GameObject>();

    private void Start()
    {
        _playerInput = transform.parent.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Interactable").performed += HammerHammer;
    }

    private void HammerHammer(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf) return;
        Camera.main.DOShakePosition(1, 1.5f);
        Camera.main.DOShakeRotation(1, 1.5f);
        
        foreach (var gameObjectInReach in _gameObjectsInTrigger)
        {
            if (!shakeEffect)
            {
                gameObjectInReach.transform.localScale -= new Vector3(0, gameObjectInReach.transform.localScale.y - 0.2f, 0);
                if (Physics.Raycast(gameObjectInReach.transform.position, Vector3.down, out var raycastHit,
                    float.PositiveInfinity))
                {
                    gameObjectInReach.transform.position = raycastHit.point + new Vector3(0,gameObjectInReach.transform.lossyScale.y / 2,0);
                }   
            }
            else gameObjectInReach.transform.DOShakeScale(1, 2f);
            
            Rigidbody rigidbody = gameObjectInReach.GetComponent<Rigidbody>();
            if (rigidbody != null)
                rigidbody.AddForce(Vector3.up * 5.0f, ForceMode.Impulse);
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
}
