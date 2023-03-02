using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
public class GameGlobals {
	private List<SpaceType> spaceTypes = new();

	private readonly List<TurnData[]> turns = new();

	public int PlayerCount => turns.Count > 0 ? turns[0].Length : 0;

	public int SpaceCount => spaceTypes.Count;

	public int TurnCount => turns.Count - 1;

	public GameGlobals(int players, IList<SpaceType> spaces) {
		Set(players, spaces);
	}

	public void Clear() {
		spaceTypes.Clear();
		turns.Clear();
	}

	public void Set(int players, IList<SpaceType> spaceTypes) {
		Clear();
		turns.Add(new TurnData[players]);
	}

	public void AddTurn() {
		turns.Add(new TurnData[PlayerCount]);
	}

	public TurnData GetTurnData(int index, Player player) {
		return turns[index][(int)player];
	}

	public void SetTurnData(int index, Player player, TurnData turnData) {
		turns[index][(int)player] = turnData;
	}

	public TurnData GetCurrentTurnData(Player player) {
		int playerId = (int)player;
		return turns.Count > 1 && turns[^1][playerId].IsEmpty
			? turns[^2][playerId]
			: turns[^1][playerId];
	}
	
	public void SetCurrentTurnData(Player player, TurnData turnData) {
		int playerId = (int)player;
		turns[^1][playerId] = turnData;
	}

	public IEnumerable<TurnData> GetAllTurnData(Player player) {
		int index = 1;
		while( index < TurnCount )
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