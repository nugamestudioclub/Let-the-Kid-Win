using System.Collections.Generic;
public static class DialogueLines {
	private static readonly Dictionary<string, Dialogue> lines;
	static DialogueLines() {
		lines = new()
		{
            { "c_Default", new (
                text: "Don't take it easy on me, Grampa.",
                player: Player.Child,
                expression: Expression.Normal  )
            },
            { "g_Default", new (
                text: "Let's have good game!",
                player: Player.Grandpa,
                expression: Expression.Normal  )
            },
            { "c_roll1", new (
                text: "Go go go!",
                player: Player.Child,
                expression: Expression.Normal  )
            },
            { "c_roll2", new (
                text: "Look at how fast I am!",
                player: Player.Child,
                expression: Expression.Normal  )
            },
            { "c_roll3", new (
                text: "My turn, my turn!",
                player: Player.Child,
                expression: Expression.Happy  )
            },
            { "c_roll4", new (
                text: "Your turn, kiddo.",
                player: Player.Grandpa,
                expression: Expression.Normal  )
            },
            { "c_roll5", new (
                text: "Good luck!",
                player: Player.Grandpa,
                expression: Expression.Normal  )
            },
            { "g_roll1", new (
                text: "Wish me luck!",
                player: Player.Grandpa,
                expression: Expression.Normal  )
            },
            { "g_roll2", new (
                text: "Good luck, Grampa!",
                player: Player.Child,
                expression: Expression.Normal  )
            },
            { "g_roll3", new (
                text: "I hope a snake doesn't bite me.",
                player: Player.Grandpa,
                expression: Expression.Normal  )
            },
            { "g_roll4", new (
                text: "Watch out for snakes!",
                player: Player.Child,
                expression: Expression.Normal  )
            },
            { "g_roll5", new (
                text: "1, 2, 3, goooo!",
                player: Player.Child,
                expression: Expression.Happy  )
            },
            { $"g_{nameof(QuestBoard.LandOnEverySnake)}", new (
				text: "How'd you manage to step on every snake?! Hahaha!",
				player: Player.Child,
				expression: Expression.Laugh  )
			},
			{ $"c_{nameof(QuestBoard.LandOnEverySnake)}", new (
				text: "I'm so unlucky, WAAAAHH!",
				player: Player.Child,
				expression: Expression.Laugh  )
			},
			{ $"g_{nameof(QuestBoard.LandOn3Snakes)}", new (
				text: "These snakes don't like me.",
				player: Player.Grandpa,
				expression: Expression.Laugh  )
			},
			{ $"c_{nameof(QuestBoard.LandOn3Snakes)}", new (
				text: "Oh no, not another one!",
				player: Player.Grandpa,
				expression: Expression.Laugh  )
			},
			{ $"g_{nameof(QuestBoard.LandOnSameSnake3Times)}", new (
				text: "Oops, this snake has it out for me.",
				player: Player.Grandpa,
				expression: Expression.Laugh  )
			},
			{ $"c_{nameof(QuestBoard.LandOnSameSnake3Times)}", new (
				text: "That snake really likes you.",
				player: Player.Grandpa,
				expression: Expression.Laugh  )
			},
			{ $"g_{nameof(QuestBoard.LandOnLadderThenSnake)}", new (
				text: "Well... easy come, easy go.",
				player: Player.Grandpa,
				expression: Expression.Happy  )
			},
			{ $"c_{nameof(QuestBoard.LandOnLadderThenSnake)}", new (
				text: "Oh noooo, I just got up here.",
				player: Player.Child,
				expression: Expression.Sad  )
			},
			{ $"g_{nameof(QuestBoard.Roll1AndLandOnSnake)}", new (
				text: "One was no good.",
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
				text: "Keep going, keep going!",
				player: Player.Grandpa,
				expression: Expression.Happy  )
			},
			{ $"g_{nameof(QuestBoard.GetAhead)}", new (
				text: "I don't want to play this game anymore...",
				player: Player.Child,
				expression: Expression.Sad  )
			},
			{ $"c_{nameof(QuestBoard.GetAhead)}", new (
				text: "Are you even trying, Grampa?",
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
				text: "Again, again, let's play again, Grampa",
				player: Player.Child,
				expression: Expression.Win  )
			},
		};
	}

	public static Dialogue GetDialogue(string key) {
		return lines.GetValueOrDefault(key,
			new(text: "My name is Child",
				player: Player.Child,
				expression: Expression.Normal));
	}
}
