using UnityEngine;
using DG.Tweening;
using Mirror;
using UnityEngine.InputSystem;

public abstract class NetworkInteractable : MonoBehaviour
{
    [Header("Generic interactable attributes")]
    //Holds information all interactable objects share like relative position etc.
    [SerializeField] protected LayerMask interactableLayers;
    [SerializeField] protected bool enableCameraShake;
    [SerializeField] protected float cameraShakeStrength;
    [SerializeField] protected float cameraShakeDuration;
    [SerializeField] protected PlayerInput playerInput;
    
    protected void ApplyCameraShake()
    {
        if (enableCameraShake)
        {
            Camera.main.DOShakePosition(cameraShakeDuration, cameraShakeStrength);
            Camera.main.DOShakeRotation(cameraShakeDuration, cameraShakeStrength);   
        }
    }
}
