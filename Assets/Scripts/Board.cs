using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Board : MonoBehaviour {
	[SerializeField]
	private bool debugMode;

	[SerializeField]
	private List<Snake> snakes = new();

	[SerializeField]
	private List<Ladder> ladders = new();

	[SerializeField]
	private List<PlayerController> gamePieces = new();

	private List<int> playerPositions = new();

	[SerializeField]
	private float moveTime =.5f;
	[SerializeField]
	private float moveSteps = 30f;

	[SerializeField]
	private List<BoardSpace> spaces;

	[SerializeField]
	private Grid grid;

	[SerializeField]
	private Vector2Int dimensions;

	[SerializeField]
	private BoardSpace spacePrefab;

	void Awake() {
		spaces = CreateAllSpaces();
		//initialize starting positions
		foreach( var piece in gamePieces ) {
			playerPositions.Add(0);
			piece.transform.position = spaces[0].transform.position;
		}
	}

	void Start() {
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
		if( player.IsMoving )
			return;
		for( int i = 1; i <= 6; ++i )
			if( Input.GetKeyDown(KeyCode.Alpha0 + i) ) {
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

	private List<BoardSpace> CreateAllSpaces() {
		return GetCellPositions().Select(cell => CreateSpace(cell.x, cell.y)).ToList();
	}

	private BoardSpace CreateSpace(int x, int y) {
		var spaceObject = Instantiate(spacePrefab, transform).GetComponent<BoardSpace>();
		spaceObject.transform.position = grid.CellToWorld(new Vector3Int(x, y, 0));
		return spaceObject;
	}

	public void MovePlayer(int playerID, int spaces) {
		if( gamePieces[playerID] != null ) {
			//run movement coroutine
			Debug.Log($"Player {playerID} is moving {spaces} spaces");
			StartCoroutine(GoMovePlayer(playerID, spaces));
		}
		else {
			Debug.Log($"Player {playerID} is not initialized");
		}
	}

	private IEnumerator GoMovePlayer(int playerID, int spaces) {
		for( int i = 0; i < spaces; i++ ) {
			yield return NextSpace(playerID);
		}
		yield return FinishMoving(playerID, playerPositions[playerID]);
	}

	private IEnumerator NextSpace(int playerID) {
		var nextSpace = spaces[playerPositions[playerID] + 1];
		var gamePiece = gamePieces[playerID];
		yield return gamePiece.MoveTo(nextSpace.transform.position, moveTime, (int)moveSteps);
		playerPositions[playerID]++;
	}

	private int FindSnakeAt(int boardIndex) {
		return snakes.FindIndex(s => s.TransportationSettings.StartIndex == boardIndex);
	}

	private int FindLadderAt(int boardIndex) {
		return ladders.FindIndex(l => l.TransportationSettings.StartIndex == boardIndex);
	}

	private IEnumerator FinishMoving(int playerID, int boardIndex) {
		Debug.Log($"Player {playerID} landed on space {boardIndex}!");
		// if space ended moving on is a shoot or ladder
		int ladderIndex = FindLadderAt(boardIndex);
		int snakeIndex = FindSnakeAt(boardIndex);
		var player = gamePieces[playerID]; 

		if( ladderIndex >= 0 ) {
			var ladder = ladders[ladderIndex];
			Debug.Log($"Player {playerID} is taking a ladder from {ladder.TransportationSettings.StartIndex} to {ladder.TransportationSettings.EndIndex}!");
			yield return player.MoveAlong(ladder.Path, ladder.TransportationSettings.DurationInSeconds, (int)moveSteps);
			playerPositions[playerID] = ladder.TransportationSettings.EndIndex;
		}
		else if( snakeIndex >= 0 ) {
			var snake = snakes[snakeIndex];
			Debug.Log($"Player {playerID} is taking a snake from {snake.TransportationSettings.StartIndex} to {snake.TransportationSettings.EndIndex}!");
			yield return player.MoveAlong(snake.Path, snake.TransportationSettings.DurationInSeconds, (int)moveSteps);
			playerPositions[playerID] = snake.TransportationSettings.EndIndex;
		}
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
			Gizmos.DrawWireSphere(point, gizmoSnakeScale);

		Gizmos.color = oldColor;
	}
#endif
}