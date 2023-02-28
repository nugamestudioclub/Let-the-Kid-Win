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
		if( IsTakingTurn && GameState.Instance.Spinner.spinFinished ) {
			IsTakingTurn = false;
			int spaces = GameState.Instance.Spinner.GetSegment();
			GameState.Instance.Board.MovePlayer(PlayerID, spaces);
		}
	}
}
