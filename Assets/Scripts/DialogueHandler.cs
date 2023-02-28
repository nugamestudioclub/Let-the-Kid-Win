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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDialogue(string s, Player player)
    {
        dialogue = new Dialogue(s, player);
        dialogueBox.SetTextBox(dialogue.player);
        dialogueBox.SetText(dialogue.text);

    }

    void ClearDialogue()
    {
        dialogueBox.Clear();
    }

    class Dialogue
    {
        public string text { get; private set; }
        public Player player { get; private set; }

        public Dialogue(string text, Player player)
        {
            this.text = text;
            this.player = player;
        }
    }
}
