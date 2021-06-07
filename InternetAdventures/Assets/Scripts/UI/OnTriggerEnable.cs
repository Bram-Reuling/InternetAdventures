using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OnTriggerEnable : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToEnable;
    [SerializeField] private LayerMask collidingLayers;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!gameObjectToEnable.activeSelf && (collidingLayers & (1 << other.gameObject.layer)) > 0)
            gameObjectToEnable.SetActive(true);
    }
}
