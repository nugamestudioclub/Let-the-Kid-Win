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

	[SerializeField]
	private float aiWaitTime;

	[field: SerializeField]
	public AudioPlayer AudioPlayer { get; private set; }

	void Awake() {
		if( Instance == null ) {
			Instance = this;
			Initialize();
		}
		else {
			Destroy(gameObject);
		}
	}

	void Update() {
		if( isfirstTime ) {
			isfirstTime = false;
			DialogueHandler.Instance.SetDialogueFromKey(
				$"g_{nameof(GameQuests.LandOnBoth)}_ladder,snake");
			NextState();
		}
	}

	private void Initialize() {
		globals.Set(players: 2, board.GetSpaceTypes(), board.LongestSnakeIndex);
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
			StartCoroutine(BeginTurn(Player.Child));
		}
		else {
			currentState = State.PlayerTurn;
			StartCoroutine(BeginTurn(Player.Grandpa));
		}
	}

	private IEnumerator BeginTurn(Player player) {
		switch( player) {
		case Player.Child:
			yield return new WaitForSeconds(aiWaitTime);
			child.TakeTurn();
			break;
		case Player.Grandpa:
			grandpa.TakeTurn();
			break;
		}
	}
}