using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : PlayerController {
	public override int PlayerID => (int)Player.Child;

	protected override void DoTakeTurn() {
		IsTakingTurn = true;
		float power = Random.Range(0.5f, 1f);
		GameState.Instance.Spinner.Spin(power);
	}

	protected override void DoUpdate() {
		var gameState = GameState.Instance;
		if( IsTakingTurn && gameState.Spinner.spinFinished ) {
			IsTakingTurn = false;
			int spaces = gameState.Spinner.GetSegment();
			gameState.Globals.LastRoll = spaces;
			gameState.Board.MovePlayer(PlayerID, spaces);
		}
	}
}
