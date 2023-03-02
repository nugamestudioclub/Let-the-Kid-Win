using System.Collections.Generic;
using System.Linq;

public static class GameQuests {
	public static Quest LandOnEach(Player player, SpaceType spaceType) {
		return new(globals => GetVisits(globals, player, spaceType).All(x => x > 0));
	}

	public static Quest RollInRange(Player player, int min, int max, int count) {
		return new(globals => {
			var turnData = globals.GetAllTurnData(player).ToList();
			return Enumerable.Range(1, count).All(x => {
				int roll = turnData[^x].Roll;
				return min <= roll && max <= roll;
			});
		});
	}

	public static Quest GetAhead(Player aheadPlayer, Player behindPlayer, int count = 1) {
		return new(globals => globals.GetCurrentSpace(aheadPlayer) >= globals.GetCurrentSpace(behindPlayer) + count);
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