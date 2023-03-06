using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour {
	[SerializeField]
	private bool debugMode;

	[SerializeField]
	private List<Snake> snakes = new();

	[SerializeField]
	private List<Ladder> ladders = new();

	[SerializeField]
	private List<PlayerController> gamePieces = new();

	private readonly List<int> playerPositions = new();

	[SerializeField]
	private float moveTime =.5f;
	[SerializeField]
	private float moveSteps = 30f;

	[SerializeField]
	private List<BoardSpace> spaces;

	[SerializeField]
	private BoardSpace spacePrefab;

	[SerializeField]
	private Grid grid;

	[SerializeField]
	private Vector2Int dimensions;

	public int Size => dimensions.x * dimensions.y;

	[field: SerializeField]
	public int LongestSnakeIndex { get; private set; } = -1;

	[SerializeField]
	private int startIndex = 0;

	void Awake() {
		spaces = CreateAllSpaces();
		//initialize starting positions)
		foreach( var piece in gamePieces ) {
			playerPositions.Add(startIndex);
			piece.transform.position = spaces[startIndex].transform.position;
		}
	}

	void Start() {
		var globals = GameState.Instance.Globals;
		for( int i = 0; i < playerPositions.Count; ++i )
			globals.SetCurrentTurnData((Player)i, new(roll: 0, startIndex));
		for( int i = 0; i < snakes.Count; ++i )
			ConnectSnake(i);
		for( int i = 0; i < ladders.Count; ++i )
			ConnectLadder(i);
	}

	void Update() {
		if( debugMode )
			DebugUpdate();
	}

	private void DebugUpdate() {
		int playerID = (int)Player.Grandpa;
		var player = gamePieces[playerID];
		if( GameState.Instance.CurrentState != GameState.State.PlayerTurn || player.IsMoving )
			return;
		for( int i = 1; i <= 6; ++i )
			if( Input.GetKeyDown(KeyCode.Alpha0 + i) ) {
				var globals = GameState.Instance.Globals;
				globals.LastRoll = i;
				MovePlayer(playerID, i);
				break;
			}
	}

	private void ConnectSnake(int index) {
		var snake = snakes[index];
		var startPoint = GetWorldPosition(snake.TransportationSettings.StartIndex);
		var endPoint = GetWorldPosition(snake.TransportationSettings.EndIndex);
		snake.ConnectPoints(startPoint, endPoint);
	}

	private void ConnectLadder(int index) {
		var ladder = ladders[index];
		var startPoint = GetWorldPosition(ladder.TransportationSettings.StartIndex);
		var endPoint = GetWorldPosition(ladder.TransportationSettings.EndIndex);
		ladder.ConnectPoints(startPoint, endPoint);
	}

	private int CellToIndex(int x, int y) {
		return (dimensions.y * y) + (y % 2 == 0 ? x : dimensions.x - x - 1);
	}

	private Vector3Int IndexToCell(int index) {
		return new(
			index / dimensions.x % 2 == 0 ? index % dimensions.x : dimensions.x - index % dimensions.x - 1,
			index / dimensions.x
		);
	}

	private Vector3 GetWorldPosition(int index) {
		return grid.CellToWorld(IndexToCell(index));
	}

	private IEnumerable<Vector3Int> GetCellPositions() {
		for( int row = 0; row < dimensions.y; ++row )
			if( row % 2 == 0 )
				for( int col = 0; col < dimensions.x; ++col )
					yield return new(col, row, 0);
			else
				for( int col = dimensions.x - 1; col >= 0; --col )
					yield return new(col, row, 0);
	}

	public IList<SpaceType> GetSpaceTypes() {
		var spaceTypes = Enumerable.Repeat(SpaceType.None, dimensions.x * dimensions.y).ToList();
		foreach( var snake in snakes )
			spaceTypes[snake.TransportationSettings.StartIndex] = SpaceType.Snake;
		foreach( var ladder in ladders )
			spaceTypes[ladder.TransportationSettings.StartIndex] = SpaceType.Ladder;
		return spaceTypes;
	}

	private List<BoardSpace> CreateAllSpaces() {
		var spaces = GetCellPositions().Select(cell => CreateSpace(cell.x, cell.y)).ToList();
		for( int i = 0; i < spaces.Count - 1; ++i )
			spaces[i].Connect(spaces[i + 1].transform.position, GetSpaceOffsetDirection(i));
		return spaces;
	}

	private Direction GetSpaceOffsetDirection(int index) {
		var cell = IndexToCell(index);
		bool isEvenRow = cell.y % 2 == 0;
		bool isLeftColumn = cell.x == 0;
		bool isRightColumn = cell.x == dimensions.x - 1;

		if( isRightColumn && isEvenRow )
			return Direction.Right;
		else if( isLeftColumn && !isEvenRow )
			return Direction.Left;
		else
			return Direction.Up;
	}

	private BoardSpace CreateSpace(int x, int y) {
		var spaceObject = Instantiate(spacePrefab, transform).GetComponent<BoardSpace>();
		spaceObject.transform.position = grid.CellToWorld(new Vector3Int(x, y, 0));
		return spaceObject;
	}

	public void MovePlayer(int playerID, int spaces) {
		if( gamePieces[playerID] != null ) {
			//run movement coroutine
			// Debug.Log($"Player {playerID} is moving {spaces} spaces");
			spaces = Math.Min(spaces, (this.spaces.Count - 1) - playerPositions[playerID]);
			StartCoroutine(GoMovePlayer(playerID, spaces));
		}
		else {
			// Debug.Log($"Player {playerID} is not initialized");
		}
	}

	private IEnumerator GoMovePlayer(int playerID, int spaces) {
		for( int i = 0; i < spaces; i++ ) {
			yield return NextSpace(playerID);
			GameState.Instance.AudioPlayer.MovePiece();
		}
		yield return FinishMoving(playerID, playerPositions[playerID]);
	}

	private IEnumerator NextSpace(int playerID) {
		var currentSpace = spaces[playerPositions[playerID]];
		var gamePiece = gamePieces[playerID];
		yield return gamePiece.MoveAlong(currentSpace.Path, moveTime, (int)moveSteps);
		playerPositions[playerID]++;
	}

	private int FindSnakeAt(int boardIndex) {
		return snakes.FindIndex(s => s.TransportationSettings.StartIndex == boardIndex);
	}

	private int FindLadderAt(int boardIndex) {
		return ladders.FindIndex(l => l.TransportationSettings.StartIndex == boardIndex);
	}

	private IEnumerator FinishMoving(int playerID, int boardIndex) {
		// Debug.Log($"Player {playerID} landed on space {boardIndex}!");
		// if space ended moving on is a shoot or ladder
		int ladderIndex = FindLadderAt(boardIndex);
		int snakeIndex = FindSnakeAt(boardIndex);
		var playerController = gamePieces[playerID];
		var player = (Player)playerID;
		var audioPlayer = GameState.Instance.AudioPlayer;
		var globals = GameState.Instance.Globals;
		var questBoard = globals.QuestBoard;
		var dlg = DialogueHandler.Instance;

		if( ladderIndex >= 0 ) {
			var ladder = ladders[ladderIndex];
			// Debug.Log($"Player {playerID} is taking a ladder from {ladder.TransportationSettings.StartIndex} to {ladder.TransportationSettings.EndIndex}!");
			audioPlayer.PlayLadder();
			yield return playerController.MoveAlong(ladder.Path, ladder.TransportationSettings.DurationInSeconds, (int)moveSteps);
			playerPositions[playerID] = ladder.TransportationSettings.EndIndex;
			audioPlayer.StopLadder();
		}
		else if( snakeIndex >= 0 ) {
			var snake = snakes[snakeIndex];
			// Debug.Log($"Player {playerID} is taking a snake from {snake.TransportationSettings.StartIndex} to {snake.TransportationSettings.EndIndex}!");
			audioPlayer.PlaySnake();
			yield return playerController.MoveAlong(snake.Path, snake.TransportationSettings.DurationInSeconds, (int)moveSteps);
			playerPositions[playerID] = snake.TransportationSettings.EndIndex;
			audioPlayer.MovePiece();
		}
		if( playerID == 0 )
			globals.AddTurn();
		globals.SetCurrentTurnData(player, new(globals.LastRoll, playerPositions[playerID]));

		var turnData = GameState.Instance.Globals.GetCurrentTurnData((Player)playerID);
		// Debug.Log($"player {playerID} roll {turnData.Roll} dst {turnData.Destination}");

		if( questBoard.Win[playerID].Evaluate(globals) ) {
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.Win));
			yield return new WaitForSeconds(4f);
			TransitionManager.ToCredits();
		}
		else {
			EvaluateDestinationQuests(player);
			GameState.Instance.NextState();
		}
	}

	private void EvaluateDestinationQuests(Player player) {
		var globals = GameState.Instance.Globals;
		var questBoard = globals.QuestBoard;
		var dlg = DialogueHandler.Instance;
		int playerId = (int)player;
		if( questBoard.LandOnLongestSnake[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.LandOnLongestSnake));
		else if( questBoard.LandOnEverySnake[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.LandOnEverySnake));
		else if( questBoard.LandOnSameSnake3Times[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.LandOnSameSnake3Times));
		else if( questBoard.LandOnLadderThenSnake[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.LandOnLadderThenSnake));
		else if( questBoard.Roll1AndLandOnSnake[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.Roll1AndLandOnSnake));
		else if( questBoard.MeetAnotherPlayer[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.MeetAnotherPlayer));
		else if( questBoard.RollHigh3Times[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.RollHigh3Times));
		else if( questBoard.GetAhead[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.GetAhead));
		else if( questBoard.LandOn3Snakes[playerId].Evaluate(globals) )
			dlg.SetDialogueFromKey(player, nameof(QuestBoard.LandOn3Snakes));
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		int index = 0;
		foreach( var cell in GetCellPositions() )
			DrawSpace(cell, index++);
		for( int i = 0; i < snakes.Count; ++i )
			DrawSnake(i);
		for( int i = 0; i < ladders.Count; ++i )
			DrawLadder(i);
	}

	[SerializeField]
	private Color gizmoSpaceColor = Color.white;

	private void DrawSpace(Vector3Int cell, int index) {

		var fill = new Color(1f, 1f, 1f, 0.1f);
		var outline = gizmoSpaceColor;
		var bounds = grid.GetBoundsLocal(cell);
		var position = grid.CellToWorld(cell);
		var rect = new Rect(position - new Vector3(bounds.extents.x, bounds.extents.y), bounds.size);
		Handles.DrawSolidRectangleWithOutline(rect, fill, outline);

		var textStyle = new GUIStyle();
		float zoom = SceneView.currentDrawingSceneView.camera.orthographicSize;
		int fontSize = 96;
		textStyle.fontStyle = FontStyle.Bold;
		textStyle.fontSize = Mathf.FloorToInt(fontSize / zoom);
		textStyle.alignment = TextAnchor.MiddleCenter;
		textStyle.normal.textColor = gizmoSpaceColor;
		Handles.Label(position, index.ToString(), textStyle);
	}

	[SerializeField]
	private float gizmoSnakeScale = 0.25f;

	[SerializeField]
	private Color gizmoSnakePrimaryColor = Color.white;

	[SerializeField]
	private Color gizmoSnakeSecondaryColor = Color.green;

	private void DrawSnake(int index) {
		var oldColor = Gizmos.color;
		Gizmos.color = gizmoSnakePrimaryColor;

		var snake = snakes[index];
		var start = GetWorldPosition(snake.TransportationSettings.StartIndex);
		var end = GetWorldPosition(snake.TransportationSettings.EndIndex);

		Gizmos.DrawWireSphere(start, gizmoSnakeScale / 2);
		foreach( var transform in snake.PointTransforms )
			Gizmos.DrawWireSphere(transform.position, gizmoSnakeScale / 2);
		Gizmos.DrawWireSphere(end, gizmoSnakeScale / 2);

		Gizmos.color = gizmoSnakeSecondaryColor;
		ConnectSnake(index);

		foreach( var point in snake.Path )
			Gizmos.DrawWireSphere(point, gizmoSnakeScale);


		Gizmos.color = oldColor;
	}

	[SerializeField]
	private Color gizmoLadderColor = Color.yellow;

	[SerializeField]
	private float gizmoLadderScale = 0.25f;

	private void DrawLadder(int index) {
		var oldColor = Gizmos.color;
		Gizmos.color = gizmoLadderColor;

		var ladder = ladders[index];
		ConnectLadder(index);
		foreach( var point in ladder.Path )
			Gizmos.DrawWireSphere(point, gizmoLadderScale);

		Gizmos.color = oldColor;
	}
#endif
}