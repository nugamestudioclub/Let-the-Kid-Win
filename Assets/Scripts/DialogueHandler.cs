using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{

    [SerializeField]
    GameObject dialogueBoxObject;
    DialogueBox dialogueBox;


    Dialogue dialogue;

    // Start is called before the first frame update
    void Start()
    {
        dialogueBox = dialogueBoxObject.GetComponent<DialogueBox>();
        // Testing
        // SetDialogue(new Dialogue("Testing...", Player.Child, Expression.Sad));
        // SetDialogue(new Dialogue("Normal Expression?", Player.Child));
        // ClearDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
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
