using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OnTriggerEnable : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToEnable;
    [SerializeField] private LayerMask collidingLayers;
    [SerializeField] private float enableTimer;
    [SerializeField] private bool turnOffAfterTimer;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObjectToEnable.activeSelf && (collidingLayers & (1 << other.gameObject.layer)) > 0)
        {
            gameObjectToEnable.SetActive(true);
            if(turnOffAfterTimer) StartCoroutine(WaitAndDisable());
        }
    }

    private IEnumerator WaitAndDisable()
    {
        gameObjectToEnable.SetActive(true);
        yield return new WaitForSeconds(enableTimer);
        gameObjectToEnable.SetActive(true);
    }
}
