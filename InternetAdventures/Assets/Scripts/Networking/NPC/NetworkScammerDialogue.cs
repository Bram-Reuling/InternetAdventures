using Mirror;
using UnityEngine;

public class NetworkScammerDialogue : NetworkBehaviour
{
    public string talkerName;
    [TextArea] public string[] dialog;
    public bool lockCharacterMovement;
    private GameObject characterMovedIn = null;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (characterMovedIn != null)
            {
                return;
            }

            characterMovedIn = other.gameObject;
            FindObjectOfType<NetworkScammerDialogueManager>().AddDialog(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
