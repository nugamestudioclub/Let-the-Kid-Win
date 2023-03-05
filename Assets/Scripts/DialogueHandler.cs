using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{

    [SerializeField]
    GameObject dialogueBoxObject;
    DialogueBox dialogueBox;

    Dialogue dialogue;

    // Start is called before the first frame update
    void Awake()
    {
        dialogueBox = dialogueBoxObject.GetComponent<DialogueBox>();
        // Testing
        // SetDialogue(new Dialogue("Testing...", Player.Child, Expression.Sad));
        // SetDialogue(new Dialogue("Normal Expression?", Player.Child));
        // ClearDialogue();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static DialogueHandler Instance;

    public void SetDialogueFromKey(string key)
    {
        SetDialogue(DialogueLines.GetDialogue(key));
    }

    public void SetDialogue(Dialogue d)
    {
        dialogue = d;
        dialogueBox.SetDialogue(d);
    }

    public void SetDialogue(string s, Player player, Expression e)
    {
        SetDialogue(new Dialogue(s, player, e));
    }

    public void SetDialogue(string s, Player player)
    {
        SetDialogue(new Dialogue(s, player));
    }

    public void ClearDialogue()
    {
        dialogueBox.Clear();
    }

}

