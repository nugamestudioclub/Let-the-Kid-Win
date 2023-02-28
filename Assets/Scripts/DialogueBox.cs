using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    Image image;
    TMPro.TextMeshProUGUI textMesh;

    [SerializeField]
    Sprite childSprite;
    [SerializeField]
    Sprite grandpaSprite;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponentInChildren<Image>();
        textMesh = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void SetTextBox(Player player)
    {
        Wake();
        switch(player)
        {
            case Player.Child:
                image.overrideSprite = childSprite;
                break;
            case Player.Grandpa:
                image.overrideSprite = grandpaSprite;
                break;
        }
    }

    /// <summary>
    /// Removes all visibility from the dialogue box
    /// </summary>
    public void Clear()
    {
        image.enabled = false;
        textMesh.enabled = false;
    }

    /// <summary>
    /// Restores visibility of the dialogue box
    /// </summary>
    void Wake()
    {
        image.enabled = true;
        textMesh.enabled = true;
    }
}
