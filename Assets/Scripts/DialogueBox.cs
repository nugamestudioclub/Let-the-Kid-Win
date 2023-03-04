using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    Image textboxImage;
    TMPro.TextMeshProUGUI textMesh;
    Image portraitImage;

    [SerializeField]
    Sprite[] childTextboxSprites;
    [SerializeField]
    Sprite[] grandpaTextboxSprites;
    [SerializeField]
    Sprite[] childPortraitSprites;
    [SerializeField]
    Sprite[] grandpaPortraitSprites;

    private Player _talkingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        textboxImage = transform.GetChild(0).GetComponent<Image>();
        textMesh = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        portraitImage = transform.GetChild(2).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets all dialogue box attributes
    /// </summary>
    public void SetDialogue(Dialogue d)
    {
        SetTextbox(d.player);
        SetText(d.text);
        SetExpression(d.player, d.expression);
    }

    /// <summary>
    /// Sets the text within the text box
    /// </summary>
    public void SetText(string s)
    {
        Wake();
        textMesh.text = s;
    }

    /// <summary>
    /// Sets the graphic of the text box
    /// </summary>
    public void SetTextbox(Player player)
    {
        Wake();
        _talkingPlayer = player;
        switch(player)
        {
            case Player.Child:
                textboxImage.overrideSprite = childTextboxSprites[1];
                break;
            case Player.Grandpa:
                textboxImage.overrideSprite = grandpaTextboxSprites[1];
                break;
        }
    }

    /// <summary>
    /// Sets the graphic of the portrait
    /// </summary>
    public void SetExpression(Player p, Expression e)
    {
        Wake();
        switch(p)
        {
            case Player.Child:
                portraitImage.overrideSprite = childPortraitSprites[(int) e];
                break;
            case Player.Grandpa:
                portraitImage.overrideSprite = grandpaPortraitSprites[((int) e)];
                break;
        }
    }

    public void SetExpression(Expression e)
    {
        SetExpression(_talkingPlayer, e);
    }

    /// <summary>
    /// Removes all visibility from the dialogue box
    /// </summary>
    public void Clear()
    {
        textboxImage.enabled = false;
        textMesh.enabled = false;
        portraitImage.enabled = false;
    }

    /// <summary>
    /// Restores visibility of the dialogue box
    /// </summary>
    void Wake()
    {
        textboxImage.enabled = true;
        textMesh.enabled = true;
        portraitImage.enabled = true;
    }
}
