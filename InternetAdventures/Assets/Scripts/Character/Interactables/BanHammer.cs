using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

public class BanHammer : Interactable
{
    [Header("Interactable-specific attributes")]
    
    //Public
    [SerializeField] private bool enableScaleEffectOnObjects;
    [SerializeField] private float animationTimer;
    
    //Private
    private readonly List<GameObject> _gameObjectsInTrigger = new List<GameObject>();
    private Vector3 _initialScale;
    private bool coroutineRunning;

    private void Start()
    {
        //Setup input
        playerInput.actions.FindAction("Interactable").performed += StartHammerCoroutine;
        _initialScale = transform.localScale;
    }

    private void StartHammerCoroutine(InputAction.CallbackContext pCallback)
    {
        if (!gameObject.activeSelf || coroutineRunning) return;
        StartCoroutine(SlamHammer());
    }

    private IEnumerator SlamHammer()
    {
        characterAnimator.SetTrigger("UseInteractable");
        coroutineRunning = true;
        yield return new WaitForSeconds(animationTimer);
        
        ApplyCameraShake();
        
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
            else
            {
                gameObjectInReach.transform.DOKill();
                gameObjectInReach.transform.localScale = _initialScale;
                gameObjectInReach.transform.DOShakeScale(1, 1.0f);
            }

            if (gameObjectInReach.CompareTag("AI"))
            {
                Destroy(gameObjectInReach.GetComponent<GoodMemberBlackboard>());
                Destroy(gameObjectInReach.GetComponent<BadMemberBlackboard>());
                gameObjectInReach.transform.GetChild(1).GetComponent<Animator>().enabled = false;
                LoseWinHandler.RemoveFromList(gameObjectInReach);
                
                //Reset tag and layer so this 'smashed' AI will not be further considered by other AIs.
                //This is especially important since the blackboard component is getting removed and will result in an 
                //exception otherwise.
                gameObjectInReach.tag = "Untagged";
                gameObjectInReach.layer = new int();
            }
            
            //Add impulse upwards if there's a rigidbody.
            Rigidbody rigidbody = gameObjectInReach.GetComponent<Rigidbody>();
            if (rigidbody != null)
                rigidbody.AddForce(Vector3.up * 5.0f, ForceMode.Impulse);
        }
        yield return null;
        yield return new WaitWhile(() => characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Interactable"));
        coroutineRunning = false;
    }
    
    //Info: The purpose of this method is to cache all gameObjects that are currently in my trigger, so I can use
    //that list when the hammer is being slammed.
    public void AddOnTriggerEnter(Collider other)
    {
        if (!_gameObjectsInTrigger.Contains(other.gameObject))
        {
            //Only add game object when its in the interactable layers.
            if ((interactableLayers.value & (1 << other.gameObject.layer)) > 0)
            {
                _gameObjectsInTrigger.Add(other.gameObject);
            }
        }
    }    
    
    //Info: The purpose here is to remove the previously added game objects from the list.
    public void RemoveOnTriggerExit(Collider other)
    {
        if(_gameObjectsInTrigger.Contains(other.gameObject)) 
            _gameObjectsInTrigger.Remove(other.gameObject);
    }
}
