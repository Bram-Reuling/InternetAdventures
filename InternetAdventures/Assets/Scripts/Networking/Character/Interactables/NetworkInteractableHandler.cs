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
    [SerializeField] private int _currentIndexInList;
    private GameObject _activeGameobject;
    private PlayerInput _playerInput;
    private float _currentScrollValue;
    [SerializeField] private GameObject initialInteractable;

    private NetworkCharacterMovement networkCharacterMovement;

    #endregion

    #region Global Functions

    

    #endregion

    #region Client Functions

    

    #endregion

    #region Server Functions

    

    #endregion
    
    private void Start()
    {
        //Setup input
        _playerInput = transform.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Scroll").performed += ChangeInteractable;

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

    private void ChangeInteractable(InputAction.CallbackContext pCallback)
    {
        //Info: Checks whether a weapon is currently in use and changes if not.
        if (networkCharacterMovement.weaponInUse) return;
        _currentScrollValue = pCallback.ReadValue<Vector2>().y;
        _currentIndexInList += _currentScrollValue < 0 ? -1 : 1;
        //This is a quick check to avoid IndexOutOfRange's
        if (_currentIndexInList < 0) _currentIndexInList = _interactables.Count - 1;
        else if (_currentIndexInList > _interactables.Count - 1) _currentIndexInList = 0;
        _activeGameobject.SetActive(false);
        _activeGameobject = _interactables.ElementAt(_currentIndexInList);
        _activeGameobject.SetActive(true);
    }
}
