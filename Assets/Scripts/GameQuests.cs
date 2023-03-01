using System.Collections.Generic;
using System.Linq;

public static class GameQuests {
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

	public static Quest HitEverySnake(Player player) {
		return new(globals => {
			var snakeVisits = GetVisits(globals, player, SpaceType.Snake);
			for( int i = 0; i < globals.SpaceCount; ++i )
				if( globals.GetTypeOfSpace(i) == SpaceType.Snake
				&& snakeVisits[i] <= 0 )
					return false;
			return true;
		});
	}
}