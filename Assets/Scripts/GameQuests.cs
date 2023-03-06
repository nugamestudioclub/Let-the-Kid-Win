using System.Collections.Generic;
using System.Linq;

public static class GameQuests {
	public static readonly Quest True = new(globals => true);

	public static readonly Quest False = new(globals => false);

	public static Quest LandOnAll(Player player, SpaceType spaceType) {
		return new(globals => GetVisits(globals, player, spaceType).All(x => x > 0));
	}

	public static Quest LandOnSame(Player player, SpaceType spaceType, int count) {
		return new(globals => GetVisits(globals, player, spaceType).Any(x => x == count));
	}

	public static Quest LandOnN(Player player, SpaceType spaceType, int count) {
		return new(globals => GetVisits(globals, player, spaceType).Count(x => x > 0) == count);
	}

	public static Quest LandOnSpace(Player player, int index) {
		return new(globals => {
			var previousTurn = globals.GetPreviousTurnData(player);
			var currentTurn = globals.GetCurrentTurnData(player);
			return previousTurn.Destination + currentTurn.Roll == index;
		});
	}

	public static Quest Destination(Player player, int index) {
		return new(globals => {
			var currentTurn = globals.GetCurrentTurnData(player);
			return currentTurn.Destination == index;
		});
	}

	public static Quest RollInRange(Player player, int min, int max, int count) {
		return new(globals => {
			var turnData = globals.GetAllTurnData(player).ToList();
			return Enumerable.Range(1, count).All(x => {
				int roll = globals.GetPreviousTurnData(player, x).Roll;
				return min <= roll && max <= roll;
			});
		});
	}

	public static Quest RollAndLandOn(Player player, int roll, SpaceType spaceType) {
		return new(globals => {
			var previousTurnData = globals.GetPreviousTurnData(player);
			var currentTurnData = globals.GetCurrentTurnData(player);
			return currentTurnData.Roll == roll
			&& globals.GetTypeOfSpace(previousTurnData.Destination + roll) == spaceType;
		});
	}

	public static Quest LandOnBoth(Player player, SpaceType spaceType1, SpaceType spaceType2) {
		return new(globals => {
			var previousTurnData2 = globals.GetPreviousTurnData(player, 2);
			var previousTurnData1 = globals.GetPreviousTurnData(player, 1);
			var currentTurnData = globals.GetCurrentTurnData(player);
			return globals.GetTypeOfSpace(previousTurnData2.Destination + previousTurnData1.Roll) == spaceType1
			 && globals.GetTypeOfSpace(previousTurnData1.Destination + currentTurnData.Roll) == spaceType2;
		});
	}

	public static Quest MeetAnotherPlayer(Player player) {
		return new(globals => {
			var previousTurnData = globals.GetPreviousTurnData(player);
			var currentTurnData = globals.GetCurrentTurnData(player);
			var spaceType = globals.GetTypeOfSpace(previousTurnData.Destination + currentTurnData.Roll);
			int dst = currentTurnData.Destination;
			int playerId = (int)player;
			if( spaceType == SpaceType.None )
				return false;
			else
				return Enumerable.Range(0, globals.PlayerCount).Any(x =>
					x != playerId
					&& globals.GetCurrentTurnData((Player)x).Destination == dst
				);
		});
	}

	public static Quest GetAhead(Player player, int count = 1) {
		return new(globals => {
			int playerId = (int)player;
			var currentSpaces = Enumerable.Range(0, globals.PlayerCount).Select(x => globals.GetCurrentSpace((Player)x)).ToList();
			for( int i = 0; i < currentSpaces.Count; ++i )
				if( currentSpaces[playerId] - count >= currentSpaces[i] )
					return true;
			return false;
		});
	}

	private static IReadOnlyList<int> GetVisits(GameGlobals globals, Player player, SpaceType spaceType) {
		var visits = Enumerable.Repeat(0, globals.SpaceCount).ToList();
		int currentSpace = 0;
		foreach( var turnData in globals.GetAllTurnData(player) ) {
			int spaceLandedOn = currentSpace + turnData.Roll;
			if( globals.GetTypeOfSpace(spaceLandedOn) == spaceType )
				++visits[spaceLandedOn];
			currentSpace = turnData.Destination;
		}
		return visits;
	}
}