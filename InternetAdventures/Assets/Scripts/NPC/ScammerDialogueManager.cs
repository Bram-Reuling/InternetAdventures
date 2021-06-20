using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScammerDialogManager : MonoBehaviour
{
    private Queue<string> dialogToShow = new Queue<string>();
    [SerializeField] private TMP_Text name, sentence;
    private CharacterMovement _characterMovement;
    private bool coroutineRunning;

    private void Start()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
    }
    
    public void AddDialog(ScammerDialog pScammerDialog)
    {
        foreach (var sentence in pScammerDialog.dialog)
        {
            dialogToShow.Enqueue(sentence);
        }

        if (pScammerDialog.lockCharacterMovement)
        {
            _characterMovement.lockMovement = true;
        }

        if (!coroutineRunning)
        {
            name.text = pScammerDialog.talkerName;
            StartCoroutine(ShowDialogue());
        }
    }


    private IEnumerator ShowDialogue()
    {
        coroutineRunning = true;
        sentence.text = dialogToShow.Peek();
        dialogToShow.Dequeue();
        yield return new WaitForSeconds(5);
        if(dialogToShow.Count > 0)
            StartCoroutine(ShowDialogue());
        else
        {
            _characterMovement.lockMovement = false;
            coroutineRunning = false;
        }
    }
}
