using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NetworkPressurePlate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.parent.GetComponent<NetworkPressurePlateHandler>().AddGameObject(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        transform.parent.parent.GetComponent<NetworkPressurePlateHandler>().RemoveGameObject(other.gameObject);
    }
    
}
