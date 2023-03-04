using System.Collections;
using System.Collections.Generic;

public class Dialogue
{
    public string text { get; private set; }
    public Player player { get; private set; }
    public Expression expression { get; private set; }

    public Dialogue(string text, Player player, Expression expression)
    {
        this.text = text;
        this.player = player;
        this.expression = expression;
    }

    public Dialogue(string text, Player player) : this(text, player, Expression.Normal)
    {

    }
}