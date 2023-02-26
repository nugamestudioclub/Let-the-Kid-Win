using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour, IPlayerController
{
    private bool isTakingTurn = false;
    public bool IsTakingTurn => isTakingTurn;

    public int PlayerID => 0;

    public void TakeTurn()
    {   
        isTakingTurn = true;
        float power = Random.Range(0, 1f);
        GameState.Instance.Spinner.Spin(power);
    }

    private void Update()
    {
        if (IsTakingTurn && GameState.Instance.Spinner.spinFinished)
        {
            isTakingTurn = false;
            int spaces = GameState.Instance.Spinner.GetSegment();
            GameState.Instance.Board.MovePlayer(PlayerID, spaces);
        }
    }
}
