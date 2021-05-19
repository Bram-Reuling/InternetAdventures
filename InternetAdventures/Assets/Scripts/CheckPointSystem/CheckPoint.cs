using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            Debug.Log("Player Collided with Check Point");
            Debug.Log("Check point position is: " + gameObject.transform.position);
            EventBroker.CallSetCheckPointEvent(gameObject.transform.position, other.name);
        }
    }
}
