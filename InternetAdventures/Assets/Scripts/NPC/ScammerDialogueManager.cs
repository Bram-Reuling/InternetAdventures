using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScammerDialogueManager : MonoBehaviour
{
    private Queue<string> dialogueToShow = new Queue<string>();
    [SerializeField] private TMP_Text name, sentence;
    private CharacterMovement _characterMovement;
    private bool coroutineRunning;

    private void Start()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
    }
    
    public void AddDialog(ScammerDialogue pScammerDialogue)
    {
        foreach (var sentence in pScammerDialogue.dialog)
        {
            dialogueToShow.Enqueue(sentence);
        }

        if (pScammerDialogue.lockCharacterMovement)
        {
            _characterMovement.UserInputAllowed = false;
        }

        if (!coroutineRunning)
        {
            name.text = pScammerDialogue.talkerName;
            StartCoroutine(ShowDialogue());
        }
    }


    private IEnumerator ShowDialogue()
    {
        coroutineRunning = true;
        sentence.text = dialogueToShow.Peek();
        dialogueToShow.Dequeue();
        yield return new WaitForSeconds(5);
        if(dialogueToShow.Count > 0)
            StartCoroutine(ShowDialogue());
        else
        {
            _characterMovement.UserInputAllowed = true;
            coroutineRunning = false;
            sentence.text = "";
        }
    }
}
