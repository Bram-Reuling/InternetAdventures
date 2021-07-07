using System;
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
    [SerializeField] private GameObject objectToDisable;

    [ServerCallback]
    public void AddDialog(NetworkScammerDialogue pScammerDialogue)
    {
        foreach (var sentence in pScammerDialogue.dialog)
        {
            dialogueToShow.Enqueue(sentence);
        }

        if (pScammerDialogue.lockCharacterMovement)
        {
            foreach (var character in GameObject.FindGameObjectsWithTag("Character"))
            {
                character.GetComponent<NetworkCharacterMovement>().UserInputAllowed = false;
            }
        }

        if (!coroutineRunning)
        {
            objectToDisable.SetActive(true);
            SetObjectActive(true);
            nameString = pScammerDialogue.talkerName;
            //Destroy(pScammerDialogue.gameObject);
            StartCoroutine(ShowDialogue());
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
            yield return new WaitForSeconds(0.05f);
        }
        
        dialogueToShow.Dequeue();
        yield return new WaitForSeconds(3.0f);
        sentenceString = " ";
        if (dialogueToShow.Count > 0)
        {
            StartCoroutine(ShowDialogue());
        }
        else
        {
            foreach (var character in GameObject.FindGameObjectsWithTag("Character"))
            {
                character.GetComponent<NetworkCharacterMovement>().UserInputAllowed = true;
            }
            coroutineRunning = false;
            sentenceString = " ";
            objectToDisable.SetActive(false);
            SetObjectActive(false);
        }
    }

    [ClientRpc]
    private void SetObjectActive(bool pValue)
    {
        objectToDisable.SetActive(pValue);
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
