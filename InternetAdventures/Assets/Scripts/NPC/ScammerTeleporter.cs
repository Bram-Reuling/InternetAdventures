using Mirror;
using UnityEngine;

public class ScammerTeleporter : NetworkBehaviour
{
    [SerializeField] private GameObject scammer;
    [SerializeField] private Transform placementPosition;
    [SerializeField] private bool showDebug;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            Physics.Raycast(placementPosition.position, Vector3.down, out RaycastHit hit, 100.0f);
            scammer.transform.position = hit.point + scammer.transform.localScale / 2;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            scammer.transform.LookAt(other.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebug) return;
        Gizmos.color = new Color(0.25f, 0.6f, 0.5f, 0.25f);
        Gizmos.DrawCube(transform.position, transform.localScale);
        Gizmos.DrawIcon(placementPosition.position, "Spawn Location");
    }
}
