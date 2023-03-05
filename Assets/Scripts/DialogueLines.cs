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
            { $"g_{nameof(QuestBoard.LandOnEverySnake)}", new (
                text: "How did you manage to step on every snake?! Hahaha!",
                player: Player.Child,
                expression: Expression.Laugh  )
            },
            { $"c_{nameof(QuestBoard.LandOnEverySnake)}", new (
                text: "I'm so unlucky, WAAAAHH!",
                player: Player.Child,
                expression: Expression.Laugh  )
            },
            { $"g_{nameof(QuestBoard.LandOnSameSnake3Times)}", new (
                text: "Oops, I guess this snakes has it out for me.",
                player: Player.Grandpa,
                expression: Expression.Laugh  )
            },
            { $"c_{nameof(QuestBoard.LandOnSameSnake3Times)}", new (
                text: "That snakes really likes you.",
                player: Player.Grandpa,
                expression: Expression.Laugh  )
            },
            { $"g_{nameof(QuestBoard.LandOnLadderThenSnake)}", new (
                text: "Well..easy come, easy go.",
                player: Player.Grandpa,
                expression: Expression.Happy  )
            },
            { $"c_{nameof(QuestBoard.LandOnLadderThenSnake)}", new (
                text: "Oh noooo, I just got up here.",
                player: Player.Child,
                expression: Expression.Sad  )
            },
            { $"g_{nameof(QuestBoard.Roll1AndLandOnSnake)}", new (
                text: "One was no good,",
                player: Player.Grandpa,
                expression: Expression.Normal  )
            },
            { $"c_{nameof(QuestBoard.Roll1AndLandOnSnake)}", new (
                text: "Aww man...",
                player: Player.Child,
                expression: Expression.Sad  )
            },
            { $"g_{nameof(QuestBoard.MeetAnotherPlayer)}", new (
                text: "Glad I could meet you here.",
                player: Player.Grandpa,
                expression: Expression.Happy  )
            },
            { $"c_{nameof(QuestBoard.MeetAnotherPlayer)}", new (
                text: "I'm now on your space, Grampa.",
                player: Player.Child,
                expression: Expression.Happy  )
            },
            { $"g_{nameof(QuestBoard.RollHigh3Times)}", new (
                text: "I'll never be that fast.",
                player: Player.Child,
                expression: Expression.Sad  )
            },
            { $"c_{nameof(QuestBoard.RollHigh3Times)}", new (
                text: "Keep going keep going!",
                player: Player.Grandpa,
                expression: Expression.Happy  )
            },
            { $"g_{nameof(QuestBoard.GetAhead)}", new (
                text: "I don't want to play this game anymore...",
                player: Player.Child,
                expression: Expression.Sad  )
            },
            { $"c_{nameof(QuestBoard.GetAhead)}", new (
                text: "Are you even trying, Grandpa?",
                player: Player.Child,
                expression: Expression.Normal  )
            },
            { $"g_{nameof(QuestBoard.LandOnLongestSnake)}", new (
                text: "",
                player: Player.Grandpa,
                expression: Expression.HitSnake  )
            },
            { $"c_{nameof(QuestBoard.LandOnLongestSnake)}", new (
                text: "",
                player: Player.Child,
                expression: Expression.HitSnake  )
            },
            { $"g_{nameof(QuestBoard.Win)}", new (
                text: "No fair, no fair...",
                player: Player.Child,
                expression: Expression.Lose  )
            },
            { $"c_{nameof(QuestBoard.Win)}", new (
                text: "Again, again, let's play again, Granpa",
                player: Player.Child,
                expression: Expression.Win  )
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
