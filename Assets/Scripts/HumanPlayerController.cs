using UnityEngine;

public class HumanPlayerController : PlayerController {
	public override int PlayerID => (int)Player.Grandpa;

	//enables using the spinner
	protected override void DoTakeTurn() {
		IsTakingTurn = true;
	}

	private bool hitSpin = false;

	protected override void DoUpdate() {
		if( IsTakingTurn && Input.GetKeyDown(KeyCode.Space) ) {
			hitSpin = true;
			float power = Random.Range(0.2f, 1f);
			GameState.Instance.Spinner.Spin(power);
		}
		else if( hitSpin && IsTakingTurn && GameState.Instance.Spinner.spinFinished ) {
			IsTakingTurn = false;
			hitSpin = false;
			int spaces = GameState.Instance.Spinner.GetSegment();
			GameState.Instance.Board.MovePlayer(PlayerID, spaces);
		}
	}
}