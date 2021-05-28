using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PressurePlate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.parent.GetComponent<PressurePlateHandler>().AddGameObject(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        transform.parent.parent.GetComponent<PressurePlateHandler>().RemoveGameObject(other.gameObject);
    }
    
}
