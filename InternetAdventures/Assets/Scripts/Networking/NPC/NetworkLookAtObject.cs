using Mirror;
using UnityEngine;

public class NetworkLookAtObject : NetworkBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject objectToLookAt;
    
    [ClientCallback]
    private void Update()
    {
        canvas.transform.LookAt(objectToLookAt.transform.position);
    }
}
