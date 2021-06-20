using System;
using UnityEngine;

public class ScammerDialogue : MonoBehaviour
{
    public string talkerName;
    [TextArea] public string[] dialog;
    public bool lockCharacterMovement;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Character"))
            FindObjectOfType<ScammerDialogueManager>().AddDialog(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
