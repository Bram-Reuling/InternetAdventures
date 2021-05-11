using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    private List<GameObject> _interactables = new List<GameObject>();
    private int _currentIndexInList;
    private GameObject _activeGameobject;
    private PlayerInput _playerInput;
    private float _currentScrollValue;
    [SerializeField] private GameObject initialInteractable;
    
    private void Start()
    {
        _playerInput = transform.parent.GetComponent<PlayerInput>();
        _playerInput.actions.FindAction("Scroll").performed += ChangeInteractable;

        int currentIndex = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject currentGameObject = transform.GetChild(i).gameObject;
            if (currentGameObject.tag.Equals("Interactable"))
            {
                currentGameObject.SetActive(currentGameObject == initialInteractable);
                if (currentGameObject == initialInteractable)
                {
                    _activeGameobject = currentGameObject;
                    currentGameObject.SetActive(true);
                }
                else currentGameObject.SetActive(false);
                _interactables.Add(currentGameObject);
                _currentIndexInList = currentIndex;
                currentIndex++;
            }
        }
    }

    private void ChangeInteractable(InputAction.CallbackContext pCallback)
    {
        if (CharacterMovement.weaponInUse) return;
        _currentScrollValue = pCallback.ReadValue<Vector2>().y;
        _currentIndexInList += _currentScrollValue < 0 ? -1 : 1;
        if (_currentIndexInList < 0) _currentIndexInList = _interactables.Count - 1;
        else if (_currentIndexInList > _interactables.Count - 1) _currentIndexInList = 0;
        _activeGameobject.SetActive(false);
        _activeGameobject = _interactables.ElementAt(_currentIndexInList);
        _activeGameobject.SetActive(true);
    }
}
