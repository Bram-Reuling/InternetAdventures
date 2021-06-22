using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{
    [SerializeField] private InteractableEnum Interactable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Character"))
        {
            other.transform.GetChild(1).GetComponent<InteractableHandler>().UnlockInteractable(Interactable);
            Destroy(gameObject);
        }
    }
}
