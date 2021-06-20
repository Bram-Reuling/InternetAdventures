using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToLookAt;
    
    private void Update()
    {
        transform.LookAt(objectToLookAt.transform.position);
    }
}
