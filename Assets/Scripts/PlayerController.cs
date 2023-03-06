using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface IPlayerController
{
    void TakeTurn();
    bool IsTakingTurn { get; }

    int PlayerID { get; }
}

public abstract class PlayerController : MonoBehaviour, IPlayerController {
	public bool IsTakingTurn { get; protected set; }

	public bool IsMoving { get; private set; }

	public abstract int PlayerID { get; }

	void Update() {
		DoUpdate();
	}

	protected virtual void DoUpdate() { }
	
	public IEnumerator MoveAlong(IReadOnlyPath<Vector3> path, float durationInSeconds, int steps) {
		IsMoving = true;
		for( int i = 0; i < path.Count - 1; i++ ) {
			float distanceToTravel = path.GetDistanceBetween(i, i + 1);
			float movementTime = (distanceToTravel / path.Length) * durationInSeconds;
			yield return MoveTo(path[i + 1], movementTime, steps);
		}
		IsMoving = false;
		yield return null;
	}

	public IEnumerator MoveTo(Vector3 endPosition, float durationInSeconds, int steps) {
		Vector3 startPosition = transform.position;
		float currentTime = 0f;
		float adjutedTime = 0f;
		float deltaTime = durationInSeconds / steps;
		float epsilon = Time.fixedDeltaTime;
		IsMoving = true;
		while( currentTime < durationInSeconds + deltaTime ) {
			transform.position = Vector2.Lerp(startPosition, endPosition, currentTime / durationInSeconds);
			adjutedTime += deltaTime;
			if( adjutedTime >= epsilon ) {
				yield return new WaitForFixedUpdate();
				adjutedTime = 0f;
			}
			currentTime += deltaTime;
		}
		IsMoving = false;
		yield return null;
	}

	public void TakeTurn() {
		int pick = Random.Range(1, 5);
		DialogueHandler.Instance.SetDialogueFromKey((Player)PlayerID, $"roll{pick}");
		DoTakeTurn();
	}

	protected abstract void DoTakeTurn();
}