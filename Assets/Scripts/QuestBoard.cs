using System.Collections.Generic;

public class QuestBoard {
	private readonly List<Entry> landOnEverySnake;
	public IReadOnlyList<Entry> LandOnEverySnake => landOnEverySnake;

	private readonly List<Entry> landOn3Snakes;
	public IReadOnlyList<Entry> LandOn3Snakes => landOn3Snakes;

	private readonly List<Entry> landOnSameSnake3Times;
	public IReadOnlyList<Entry> LandOnSameSnake3Times => landOnSameSnake3Times;

	private readonly List<Entry> landOnLadderThenSnake;
	public IReadOnlyList<Entry> LandOnLadderThenSnake => landOnLadderThenSnake;

	private readonly List<Entry> roll1AndLandOnSnake;
	public IReadOnlyList<Entry> Roll1AndLandOnSnake => roll1AndLandOnSnake;

	private readonly List<Entry> meetAnotherPlayer;
	public IReadOnlyList<Entry> MeetAnotherPlayer => meetAnotherPlayer;

	private readonly List<Entry> rollHigh3Times;
	public IReadOnlyList<Entry> RollHigh3Times => rollHigh3Times;

	private readonly List<Entry> getAhead;
	public IReadOnlyList<Entry> GetAhead => getAhead;

	private readonly List<Entry> landOnLongestSnake;
	public IReadOnlyList<Entry> LandOnLongestSnake => landOnLongestSnake;

	private readonly List<Entry> win;
	public IReadOnlyList<Entry> Win => win;

	public QuestBoard(int numberOfPlayers, int boardSize, int longestSnakeIndex) {
		win = new(numberOfPlayers);
		landOnEverySnake = new(numberOfPlayers);
		landOn3Snakes = new(numberOfPlayers);
		landOnSameSnake3Times = new(numberOfPlayers);
		landOnLadderThenSnake = new(numberOfPlayers);
		roll1AndLandOnSnake = new(numberOfPlayers);
		meetAnotherPlayer = new(numberOfPlayers);
		rollHigh3Times = new(numberOfPlayers);
		getAhead = new(numberOfPlayers);
		landOnLongestSnake = new(numberOfPlayers);

		for( int i = 0; i < numberOfPlayers; ++i ) {
			var player = (Player)i;
			landOnEverySnake.Add(new(GameQuests.LandOnAll(player, SpaceType.Snake)));
			landOn3Snakes.Add(new(GameQuests.LandOnN(player, SpaceType.Snake, count: 3)));
			landOnSameSnake3Times.Add(new(GameQuests.LandOnSame(player, SpaceType.Snake, count: 3)));
			landOnLadderThenSnake.Add(new(GameQuests.LandOnBoth(player, SpaceType.Ladder, SpaceType.Snake), maxTimesCompleted: int.MaxValue));
			roll1AndLandOnSnake.Add(new(GameQuests.RollAndLandOn(player, roll: 1, SpaceType.Snake), maxTimesCompleted: int.MaxValue));
			meetAnotherPlayer.Add(new(GameQuests.MeetAnotherPlayer(player), maxTimesCompleted: int.MaxValue));
			rollHigh3Times.Add(new(GameQuests.RollInRange(player, min: 5, max: 6, count: 3), maxTimesCompleted: int.MaxValue));
			getAhead.Add(new(GameQuests.GetAhead(Player.Child, count: boardSize / 2), int.MaxValue));
			landOnLongestSnake.Add(new(GameQuests.LandOnSpace(player, longestSnakeIndex), int.MaxValue));
			win.Add(new(GameQuests.Destination(player, index: boardSize - 1)));
		}
	}

	public class Entry {
		private readonly Quest quest;
		public Quest Quest => quest;

		private readonly int maxTimesCompleted;

		private int timesCompleted;

		public Entry(Quest quest, int maxTimesCompleted = 1) {
			this.quest = quest;
			this.maxTimesCompleted = maxTimesCompleted;
		}

		public bool Evaluate(GameGlobals globals) {
			if( timesCompleted < maxTimesCompleted && quest.IsComplete(globals) ) {
				if( maxTimesCompleted != int.MaxValue )
					++timesCompleted;
				return true;
			}
			else {
				return false;
			}
		}
	}
}