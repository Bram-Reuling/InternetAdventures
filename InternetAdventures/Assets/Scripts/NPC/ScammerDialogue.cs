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
}
