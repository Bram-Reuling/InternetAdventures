using System.Collections;
using System.Collections.Generic;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;

public class NetworkScammerDialogueManager : NetworkBehaviour
{
    private Queue<string> dialogueToShow = new Queue<string>();
    [SerializeField] private TMP_Text name;
    [SyncVar(hook = nameof(SetName))] private string nameString;
    [SerializeField] private TMP_Text sentence;
    [SyncVar(hook = nameof(SetSentence))] private string sentenceString;
    private NetworkCharacterMovement _characterMovement;
    private bool coroutineRunning;

    [ServerCallback]
    private void Start()
    {
        _characterMovement = FindObjectOfType<NetworkCharacterMovement>();
    }
    
    [ServerCallback]
    public void AddDialog(NetworkScammerDialogue pScammerDialogue)
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
            nameString = pScammerDialogue.talkerName;
            StartCoroutine(ShowDialogue());
        }
    }

    [ServerCallback]
    private IEnumerator ShowDialogue()
    {
        coroutineRunning = true;
        sentenceString = dialogueToShow.Peek();
        dialogueToShow.Dequeue();
        yield return new WaitForSeconds(5);
        if(dialogueToShow.Count > 0)
            StartCoroutine(ShowDialogue());
        else
        {
            _characterMovement.UserInputAllowed = true;
            coroutineRunning = false;
            sentenceString = "";
        }
    }

    [ClientCallback]
    private void SetName(string oldName, string newName)
    {
        name.text = newName;
    }

    [ClientCallback]
    private void SetSentence(string oldSentence, string newSentence)
    {
        sentence.text = newSentence;
    }
}