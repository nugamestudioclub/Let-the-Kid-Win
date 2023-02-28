using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayerController : MonoBehaviour, IPlayerController
{
    private bool isTakingTurn = false;
    public bool IsTakingTurn => isTakingTurn;

    public int PlayerID => 1;

    //enables using the spinner
    public void TakeTurn()
    {
        isTakingTurn = true;
    }

    private bool hitSpin = false;

    private void Update()
    {
        if (isTakingTurn && Input.GetKeyDown(KeyCode.Space)) {
            hitSpin = true;
            float power = Random.Range(0.2f, 1f);
            GameState.Instance.Spinner.Spin(power);
        }
        else if (hitSpin && IsTakingTurn && GameState.Instance.Spinner.spinFinished)
        {
            isTakingTurn=false;
            hitSpin=false;
            int spaces = GameState.Instance.Spinner.GetSegment();
            GameState.Instance.Board.MovePlayer(PlayerID, spaces);
        }
    }
}
