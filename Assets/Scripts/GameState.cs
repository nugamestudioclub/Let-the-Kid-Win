using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameState : MonoBehaviour {
	public static GameState Instance;

	private readonly GameGlobals globals = new();
	public GameGlobals Globals => globals;

	[SerializeField]
	private State currentState;
	public State CurrentState { get => currentState; }

	private bool isfirstTime = true;

	[SerializeField]
	private Spinner spinner;
	public Spinner Spinner => spinner;

	[SerializeField]
	private ChargePower chargePower;
	public ChargePower ChargePower => chargePower;

	[SerializeField]
	private Board board;
	public Board Board => board;

	[SerializeField]
	private AIPlayerController child;

	[SerializeField]
	private HumanPlayerController grandpa;

	void Awake() {
		if( Instance == null ) {
			Instance = this;
			Initialize();
			DontDestroyOnLoad(gameObject);

		}
		else {
			Destroy(gameObject);
		}
	}

	void Update() {
		if( isfirstTime ) {
			isfirstTime = false;
			NextState();
		}
	}

	private void Initialize() {
		globals.Set(players: 2, board.GetSpaceTypes());
	}

	public enum State {
		PlayerTurn,
		AITurn,
		DialoguePlayer,
		DialogueAI,
	}

	public void NextState() {
		if( currentState == State.PlayerTurn ) {
			currentState = State.AITurn;
			child.TakeTurn();
		}
		else {
			currentState = State.PlayerTurn;
			grandpa.TakeTurn();
		}
	}
}