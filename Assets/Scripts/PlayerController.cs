using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController
{
    void TakeTurn();
    bool IsTakingTurn { get; }

    int PlayerID { get; }
}
