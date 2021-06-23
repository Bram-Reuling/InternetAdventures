using Mirror;
using UnityEngine;

public class NetworkInteractableTrigger : NetworkBehaviour
{
    [SerializeField] private InteractableEnum Interactable;
    [SerializeField] private GameObject pickUpInteractable;
    private int timesPickedUp;
    private GameObject _firstCharacter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Character"))
        {
            
            other.transform.GetComponent<NetworkInteractableHandler>().UnlockInteractable(Interactable);

            if (_firstCharacter == null || _firstCharacter != other.gameObject) 
            {
                _firstCharacter = other.gameObject;
                timesPickedUp++;
            }

            if (timesPickedUp > 1)
            {
                Destroy(pickUpInteractable);
                Destroy(gameObject);
            }
        }
    }
}
