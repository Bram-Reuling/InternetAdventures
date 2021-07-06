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
    public void AddDialog(NetworkScammerDialogue pScammerDialogue)
    {
        foreach (var sentence in pScammerDialogue.dialog)
        {
            dialogueToShow.Enqueue(sentence);
        }

        if (pScammerDialogue.lockCharacterMovement)
        {
            if(_characterMovement == null)
                _characterMovement = FindObjectOfType<NetworkCharacterMovement>();
            _characterMovement.UserInputAllowed = false;
        }

        if (!coroutineRunning)
        {
            nameString = pScammerDialogue.talkerName;
            //Destroy(pScammerDialogue.gameObject);
            StartCoroutine(ShowDialogue());
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    [ServerCallback]
    private IEnumerator ShowDialogue()
    {
        coroutineRunning = true;
        char[] stringToCharArray = dialogueToShow.Peek().ToCharArray();
        foreach (var letter in stringToCharArray)
        {
            sentenceString += letter;
            yield return new WaitForSeconds(0.03f);
        }
        
        dialogueToShow.Dequeue();
        yield return new WaitForSeconds(2);
        if (dialogueToShow.Count > 0)
        {
            StartCoroutine(ShowDialogue());
        }
        else
        {
            if(_characterMovement != null)
                _characterMovement.UserInputAllowed = true;
            coroutineRunning = false;
            sentenceString = " ";
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
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
