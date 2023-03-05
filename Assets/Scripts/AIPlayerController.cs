using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : PlayerController {
	public override int PlayerID => (int)Player.Child;

	private float power;
	private bool isCharging = false;

	protected override void DoTakeTurn() {
		IsTakingTurn = true;
		isCharging = true;
		power = Random.Range(0.3f, 1f);
	}

	protected override void DoUpdate() {
		var gameState = GameState.Instance;
		if (IsTakingTurn)
        {
			if (isCharging)
            {
				gameState.ChargePower.Tick();
				if (gameState.ChargePower.GetCharge() > power)
                {
					isCharging = false;
					GameState.Instance.Spinner.Spin(power);
				}
            }
			else if (gameState.Spinner.spinFinished)
            {
				IsTakingTurn = false;
				gameState.ChargePower.ResetCharge();
				int spaces = gameState.Spinner.GetSegment();
				gameState.Globals.LastRoll = spaces;
				gameState.Board.MovePlayer(PlayerID, spaces);
			}
        }


		/* if( IsTakingTurn && gameState.Spinner.spinFinished ) {
			IsTakingTurn = false;
			int spaces = gameState.Spinner.GetSegment();
			gameState.Globals.LastRoll = spaces;
			gameState.Board.MovePlayer(PlayerID, spaces);
		} */
	}
}
