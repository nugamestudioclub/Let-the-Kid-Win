using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
public class GameGlobals {
	private readonly List<SpaceType> spaceTypes = new();

	private readonly List<TurnData[]> turns = new();

	public int PlayerCount => turns.Count > 0 ? turns[0].Length : 0;

	public int SpaceCount => spaceTypes.Count;

	public int LastRoll { get; set; }

	public QuestBoard QuestBoard { get; private set; }

	public GameGlobals() {
		Set(0, Enumerable.Empty<SpaceType>().ToList(), -1);
	}

	public GameGlobals(int players, IList<SpaceType> spaces, int longestSnakeIndex) {
		Set(players, spaces, longestSnakeIndex);
	}

	public void Clear() {
		spaceTypes.Clear();
		turns.Clear();
		QuestBoard = new(numberOfPlayers: 0, boardSize: 0, longestSnakeIndex: -1);
	}

	public void Set(int players, IList<SpaceType> spaceTypes, int longestSnakeIndex) {
		Clear();
		turns.Add(new TurnData[players]);
		this.spaceTypes.AddRange(spaceTypes);
		QuestBoard = new(players, spaceTypes.Count, longestSnakeIndex);
	}

	public void AddTurn() {
		turns.Add(new TurnData[PlayerCount]);
	}

	public int CountTurns(Player player) {
		int playerId = (int)player;
		return turns.Count > 1 && turns[^1][playerId].IsEmpty
			? turns.Count - 2
			: turns.Count - 1;
	}

	public TurnData GetTurnData(int index, Player player) {
		return turns[index][(int)player];
	}

	public void SetTurnData(int index, Player player, TurnData turnData) {
		turns[index][(int)player] = turnData;
	}

	public TurnData GetCurrentTurnData(Player player) {
		return turns[CountTurns(player)][(int)player];
	}

	public void SetCurrentTurnData(Player player, TurnData turnData) {
		int playerId = (int)player;
		turns[^1][playerId] = turnData;
	}

	public TurnData GetPreviousTurnData(Player player, int count = 1) {
		int index = CountTurns(player);
		int playerId = (int)player;
		return count < index
			? turns[index - count][playerId]
			: turns[0][playerId];
	}

	public IEnumerable<TurnData> GetAllTurnData(Player player) {
		int index = 1;
		while( index < turns.Count - 1 )
			yield return GetTurnData(index++, player);
		var lastTurnData = GetTurnData(index, player);
		if( !lastTurnData.IsEmpty )
			yield return lastTurnData;
	}

	public int GetCurrentSpace(Player player) {
		return GetCurrentTurnData(player).Destination;
	}

	public SpaceType GetTypeOfSpace(int index) {
		return spaceTypes[index];
	}

	public IReadOnlyList<SpaceType> SpaceTypes => spaceTypes;
}