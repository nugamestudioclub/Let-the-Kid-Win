using System.Collections.Generic;

public static class DialogueLines
{
    private static readonly Dictionary<string, Dialogue> lines;
    static DialogueLines()
    {
        lines = new()
        {
            { "c_Default", new (
                text: "My name is Child.",
                player: Player.Child,
                expression: Expression.Normal  )
            },
            { "g_Default", new (
                text: "My name is Grandpa.",
                player: Player.Grandpa,
                expression: Expression.Normal  )
            },
            { $"g_{nameof(GameQuests.LandOnAll)}_snakes", new (
                text: "How did you manage to step on every snake?! Hahaha!",
                player: Player.Child,
                expression: Expression.Laugh  )
            },
            { $"c_{nameof(GameQuests.LandOnAll)}_snakes", new (
                text: "I'm so unlucky, WAAAAHH!",
                player: Player.Child,
                expression: Expression.Laugh  )
            },
            { $"g_{nameof(GameQuests.LandOnSame)}_snake3", new (
                text: "Oops, I guess this snakes has it out for me.",
                player: Player.Grandpa,
                expression: Expression.Laugh  )
            },
            { $"c_{nameof(GameQuests.LandOnSame)}_snake3", new (
                text: "That snakes really likes you.",
                player: Player.Grandpa,
                expression: Expression.Laugh  )
            },
            { $"g_{nameof(GameQuests.LandOnBoth)}_ladder,snake", new (
                text: "Well..easy come, easy go.",
                player: Player.Grandpa,
                expression: Expression.Happy  )
            },
            { $"c_{nameof(GameQuests.LandOnBoth)}_ladder,snake", new (
                text: "Oh noooo, I just got up here.",
                player: Player.Child,
                expression: Expression.Sad  )
            },
        };
    }

    public static Dialogue GetDialogue(string key)
    {
        return lines.GetValueOrDefault(key,
            new(text: "My name is Child",
                player: Player.Child,
                expression: Expression.Normal));
    }
}
