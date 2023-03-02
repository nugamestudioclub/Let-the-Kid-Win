using UnityEngine;

public class HumanPlayerController : PlayerController {
	public override int PlayerID => (int)Player.Grandpa;

	//enables using the spinner
	protected override void DoTakeTurn() {
		IsTakingTurn = true;
	}

	private bool hitSpin = false;

	protected override void DoUpdate() {
		var gameState = GameState.Instance;
		if( IsTakingTurn && Input.GetKeyDown(KeyCode.Space) ) {
			hitSpin = true;
			float power = Random.Range(0.2f, 1f);
			gameState.Spinner.Spin(power);
		}
		else if( hitSpin && IsTakingTurn && GameState.Instance.Spinner.spinFinished ) {
			IsTakingTurn = false;
			hitSpin = false;
			int spaces = gameState.Spinner.GetSegment();
			gameState.Globals.LastRoll = spaces;
			gameState.Board.MovePlayer(PlayerID, spaces);
		}
	}
}