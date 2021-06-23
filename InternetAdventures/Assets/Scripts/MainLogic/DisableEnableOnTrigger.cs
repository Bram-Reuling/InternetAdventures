using Mirror;
using UnityEngine;

public class DisableEnableOnTrigger : NetworkBehaviour
{
    [SerializeField] private GameObject[] toEnable;
    [SerializeField] private GameObject[] toDisable;
    [SerializeField] private bool destroy;
    
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            foreach (var enableGO in toEnable)
            {
                if (destroy)
                {
                    Destroy(enableGO);
                    return;
                }
                enableGO.SetActive(true);
            }

            foreach (var disableGO in toDisable)
            {
                if (destroy)
                {
                    Destroy(disableGO);
                    return;
                }
                disableGO.SetActive(false);
            }
            
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    private void RPCOnTriggerEnter()
    {
        foreach (var enableGO in toEnable)
        {
            if (destroy)
            {
                Destroy(enableGO);
                return;
            }
            enableGO.SetActive(true);
        }

        foreach (var disableGO in toDisable)
        {
            if (destroy)
            {
                Destroy(disableGO);
                return;
            }
            disableGO.SetActive(false);
        }
            
        Destroy(gameObject);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 0.2f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
