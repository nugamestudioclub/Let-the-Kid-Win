using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest {
	private readonly Func<GameGlobals, bool> IsCompleteCallback;

	public Quest(Func<GameGlobals, bool> IsCompleteCallback) {
		this.IsCompleteCallback = IsCompleteCallback;
	}

	public bool IsComplete(GameGlobals gameGlobals) {
		return IsCompleteCallback(gameGlobals);
	}
}
