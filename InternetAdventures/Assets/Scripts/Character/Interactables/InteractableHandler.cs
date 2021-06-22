using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableHandler : MonoBehaviour
{
    private List<GameObject> _interactables = new List<GameObject>();
    private int _currentIndexInList;
    private GameObject _activeGameobject;
    [SerializeField]private PlayerInput playerInput;
    private float _currentScrollValue;
    [SerializeField] private GameObject initialInteractable;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private AnimatorOverrideController handsAnimatorOverrideController;
    [SerializeField] private AnimatorOverrideController gunAnimatorOverrideController;
    [SerializeField] private AnimatorOverrideController banHammerAnimatorOverrideController;
    
    private void Start()
    {
        //Setup input
        playerInput.actions.FindAction("Scroll").performed += ChangeInteractable;

        //Iterates through all children and saves all with the tag 'Interactable' in a list to scroll through
        int currentIndex = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject currentGameObject = transform.GetChild(i).gameObject;
            if (currentGameObject.tag.Equals("Interactable"))
            {
                if (currentGameObject == initialInteractable)
                {
                    _activeGameobject = currentGameObject;
                    _currentIndexInList = currentIndex;
                    currentGameObject.SetActive(true);
                    currentGameObject.GetComponent<Interactable>().UnlockWeapon();
                }
                else currentGameObject.SetActive(false);
                _interactables.Add(currentGameObject);
                currentIndex++;
            }
        }
    }

    public void UnlockInteractable(InteractableEnum pInteractableEnum)
    {
        foreach (var interactable in _interactables)
        {
            if (interactable.GetComponent<Interactable>().interactableType == pInteractableEnum)
            {
                interactable.GetComponent<Interactable>().UnlockWeapon();
            }
        }
    }

    private void ChangeInteractable(InputAction.CallbackContext pCallback)
    {
        //Info: Checks whether a weapon is currently in use and changes if not.
        if (CharacterMovement.weaponInUse) return;
        _currentScrollValue = pCallback.ReadValue<Vector2>().y;
        do
        {
            _currentIndexInList += _currentScrollValue < 0 ? -1 : 1;
            //This is a quick check to avoid IndexOutOfRange's
            if (_currentIndexInList < 0) _currentIndexInList = _interactables.Count - 1;
            else if (_currentIndexInList > _interactables.Count - 1) _currentIndexInList = 0;
        } while (_interactables.ElementAt(_currentIndexInList).GetComponent<Interactable>().IsLocked);

        _activeGameobject.SetActive(false);
        _activeGameobject = _interactables.ElementAt(_currentIndexInList);
        _activeGameobject.SetActive(true);
        SetAnimatorOverride(_activeGameobject.name);
    }

    private void SetAnimatorOverride(in string pInteractableName)
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
}

public enum InteractableEnum
{
    Hands,
    GravityGun,
    ShockwaveGun,
    BanHammer
}
