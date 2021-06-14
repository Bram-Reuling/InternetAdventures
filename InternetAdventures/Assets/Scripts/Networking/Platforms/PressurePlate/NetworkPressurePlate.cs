using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NetworkPressurePlate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered pressure plate");
        transform.parent.parent.GetComponent<NetworkPressurePlateHandler>().AddGameObject(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Left pressure plate");
        transform.parent.parent.GetComponent<NetworkPressurePlateHandler>().RemoveGameObject(other.gameObject);
    }
    
}
