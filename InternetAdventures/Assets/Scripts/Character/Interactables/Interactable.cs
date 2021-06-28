using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public abstract class Interactable : MonoBehaviour
{
    [Header("Generic interactable attributes")]
    //Holds information all interactable objects share like relative position etc.
    [SerializeField] protected LayerMask interactableLayers;
    [SerializeField] protected bool enableCameraShake;
    [SerializeField] protected float cameraShakeStrength;
    [SerializeField] protected float cameraShakeDuration;
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] protected Animator characterAnimator;
    public InteractableEnum interactableType;
    public bool IsLocked { get; private set; } = true;
    [SerializeField] private bool isLocked = true;
    

    protected void ApplyCameraShake()
    {
        if (enableCameraShake)
        {
            Camera.main.DOShakePosition(cameraShakeDuration, cameraShakeStrength);
            Camera.main.DOShakeRotation(cameraShakeDuration, cameraShakeStrength);   
        }
    }

    public void UnlockWeapon()
    {
        Debug.Log("Unlocking weapon.");
        IsLocked = false;
    }
}
