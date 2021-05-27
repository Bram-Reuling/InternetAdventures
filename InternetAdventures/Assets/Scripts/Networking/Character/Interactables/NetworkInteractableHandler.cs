using System.Collections.Generic;
using System.Linq;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkInteractableHandler : NetworkBehaviour
{
    #region Variables

    private SyncList<GameObject> _interactables = new SyncList<GameObject>();
    [SerializeField, SyncVar] private int _currentIndexInList;
    private GameObject _activeGameobject;
    private PlayerInput _playerInput;
    private float _currentScrollValue;
    [SerializeField] private GameObject initialInteractable;

    private NetworkCharacterMovement networkCharacterMovement;

    #endregion

    #region Global Functions

    private void Start()
    {
        ClientStart();

        ServerStart();
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
    
    #endregion

    #region Server Functions

    [ServerCallback]
    private void ServerStart()
    {
        networkCharacterMovement = transform.GetComponent<NetworkCharacterMovement>();

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
                }
                else currentGameObject.SetActive(false);

                _interactables.Add(currentGameObject);
                currentIndex++;
            }
        }
    }

    [Command]
    private void CmdChangeInteractable(float scrollValue)
    {
        Debug.Log("Scroll");
        //Info: Checks whether a weapon is currently in use and changes if not.
        if (networkCharacterMovement.weaponInUse) return;
        _currentIndexInList += scrollValue < 0 ? -1 : 1;
        //This is a quick check to avoid IndexOutOfRange's
        if (_currentIndexInList < 0) _currentIndexInList = _interactables.Count - 1;
        else if (_currentIndexInList > _interactables.Count - 1) _currentIndexInList = 0;
        _activeGameobject.SetActive(false);
        _activeGameobject = _interactables.ElementAt(_currentIndexInList);
        _activeGameobject.SetActive(true);
    }
    
    #endregion
}
