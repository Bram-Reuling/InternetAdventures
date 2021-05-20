using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //Holds information all interactable objects share like relative position etc.
    [SerializeField] protected LayerMask interactableLayers;
    [SerializeField] protected bool enableCameraShake;
    [SerializeField] protected float cameraShakeStrength;
    [SerializeField] protected float cameraShakeDuration;
}
