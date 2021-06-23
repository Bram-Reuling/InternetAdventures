using System.Collections.Generic;
using System.Linq;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkInteractableHandler : NetworkBehaviour
{
    #region Variables

    private List<GameObject> _interactables = new List<GameObject>();
    [SerializeField, SyncVar(hook = nameof(SetIndexInList))] private int _currentIndexInList;
    private GameObject _activeGameobject = null;
    private PlayerInput _playerInput;
    private float _currentScrollValue;
    [SerializeField] private GameObject initialInteractable;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private AnimatorOverrideController handsAnimatorOverrideController;
    [SerializeField] private AnimatorOverrideController gunAnimatorOverrideController;
    [SerializeField] private AnimatorOverrideController banHammerAnimatorOverrideController;
    private NetworkCharacterMovement networkCharacterMovement;

    #endregion

    #region Global Functions

    private void UnlockWeapon(GameObject pWeapon)
    {
        Debug.Log("Unlocking: " + pWeapon.name);
        pWeapon.GetComponent<NetworkInteractable>().UnlockWeapon();
    }
    
    private void Start()
    {
        _activeGameobject = initialInteractable;
        
        ClientStart();

        ServerStart();
        
        //Iterates through all children and saves all with the tag 'Interactable' in a list to scroll through
        int currentIndex = 0;
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            GameObject currentGameObject = transform.GetChild(0).GetChild(i).gameObject;
            if (currentGameObject.tag.Equals("Interactable"))
            {
                if (currentGameObject == initialInteractable)
                {
                    _activeGameobject = currentGameObject;
                    _currentIndexInList = currentIndex;
                    currentGameObject.SetActive(true);
                    UnlockWeapon(currentGameObject);
                }
                else currentGameObject.SetActive(false);

                _interactables.Add(currentGameObject);
                currentIndex++;
            }
        }
    }

    private void SetAnimatorLayer(in string pInteractableName)
    {
        switch (pInteractableName)
        {
            case "Hands":
                characterAnimator.runtimeAnimatorController = handsAnimatorOverrideController;
                break;
            case "BanHammer":
                characterAnimator.runtimeAnimatorController = banHammerAnimatorOverrideController;
                break;
            default:
                characterAnimator.runtimeAnimatorController = gunAnimatorOverrideController;
                break;
        }
    }
    
    #endregion

    #region Client Functions

    [ClientCallback]
    private void ClientStart()
    {
        if (!hasAuthority) return;
        //Setup input
        _playerInput = transform.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Scroll").performed += ChangeInteractable;
    }

    [ClientCallback]
    private void ChangeInteractable(InputAction.CallbackContext pCallback)
    {
        Debug.Log("Request Scroll");
        _currentScrollValue = pCallback.ReadValue<Vector2>().y;
        CmdChangeInteractable(_currentScrollValue);
    }
    
    [ClientCallback]
    private void SetIndexInList(int oldIndex, int newIndex)
    {
        _currentIndexInList = newIndex;
    }

    [TargetRpc]
    private void RpcUnlockWeapon(int pWeaponIndex)
    {
        Debug.Log("Unlocking weapon at index:" + pWeaponIndex);
        UnlockWeapon(_interactables[pWeaponIndex]);
    }
    #endregion

    #region Server Functions

    [ServerCallback]
    private void ServerStart()
    {
        networkCharacterMovement = transform.GetComponent<NetworkCharacterMovement>();
    }

    [Command]
    private void CmdChangeInteractable(float scrollValue)
    {
        //Debug.Log("Scroll");
        //Info: Checks whether a weapon is currently in use and changes if not.
        if (networkCharacterMovement.weaponInUse) return;

        do
        {
            _currentIndexInList += scrollValue < 0 ? -1 : 1;
            //This is a quick check to avoid IndexOutOfRange's
            if (_currentIndexInList < 0) _currentIndexInList = _interactables.Count - 1;
            else if (_currentIndexInList > _interactables.Count - 1) _currentIndexInList = 0;   
        } while (_interactables.ElementAt(_currentIndexInList).GetComponent<NetworkInteractable>().IsLocked);

        _activeGameobject.SetActive(false);
        _activeGameobject = _interactables.ElementAt(_currentIndexInList);
        _activeGameobject.SetActive(true);
        SetAnimatorLayer(_activeGameobject.name);
        
        RpcSetNewActiveGameObject();
    }

    [ClientRpc]
    private void RpcSetNewActiveGameObject()
    {
        _activeGameobject.SetActive(false);
        _activeGameobject = _interactables.ElementAt(_currentIndexInList);
        _activeGameobject.SetActive(true);
        SetAnimatorLayer(_activeGameobject.name);
    }

    [ServerCallback]
    public void UnlockInteractable(InteractableEnum pInteractableEnum)
    {
        int index = 0;
        Debug.Log("Unlock Interactable called for: " + pInteractableEnum);
        foreach (GameObject interactable in _interactables)
        {
            if (interactable.GetComponent<NetworkInteractable>().interactableType == pInteractableEnum)
            {
                interactable.GetComponent<NetworkInteractable>().UnlockWeapon();
                RpcUnlockWeapon(index);
            }

            index++;
        }
    }
    
    #endregion
}