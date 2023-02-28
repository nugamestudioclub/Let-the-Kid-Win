using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Board : MonoBehaviour {
	[SerializeField]
	private List<Transporter> snakes = new();

	[SerializeField]
	private List<Transporter> ladders = new();

	[SerializeField]
	private List<GameObject> gamePieces = new();

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
		foreach( GameObject piece in gamePieces ) {
			playerPositions.Add(0);
			piece.transform.position = spaces[0].transform.position;
		}
	}

	void Start() {
		for( int i = 0; i < snakes.Count; ++i )
			ConnectSnake(i);
	}

	void Update() {
		for( int i = 1; i <= 6; ++i )
			if( Input.GetKeyDown(KeyCode.Alpha0 + i) ) {
				MovePlayer(0, i);
				break;
			}
	}

	private void ConnectSnake(int index) {
		var snake = snakes[index];
		var start = GetWorldPosition(snake.StartIndex);
		var end = GetWorldPosition(snake.EndIndex);
		snake.ConnectPoints(start, end);
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
		var currentSpace = spaces[playerPositions[playerID]];
		var nextSpace = spaces[playerPositions[playerID] + 1];
		var gamePiece = gamePieces[playerID];
		yield return MoveBetween(gamePiece.transform, currentSpace.transform.position, nextSpace.transform.position, moveTime);
		playerPositions[playerID]++;
	}

	private IEnumerator MoveBetween(Transform transform, Vector2 startPoint, Vector2 endPoint, float moveTime) {
		float adjustedMoveTime = 0;
		float moveStep = moveTime / moveSteps;
		for( float i = 0; i < (moveTime + moveStep); i += moveStep ) {
			transform.position = Vector2.Lerp(startPoint, endPoint, i / moveTime);
			adjustedMoveTime += moveStep;
			if( adjustedMoveTime >= Time.fixedDeltaTime ) {
				yield return new WaitForSeconds(Time.fixedDeltaTime);
				adjustedMoveTime = 0;
			}
		}
		yield return null;
	}

	private int FindSnakeAt(int boardIndex) {
		return snakes.FindIndex(s => s.StartIndex == boardIndex);
	}

	private int FindLadderAt(int boardIndex) {
		return ladders.FindIndex(l => l.StartIndex == boardIndex);
	}

	private IEnumerator FinishMoving(int playerID, int boardIndex) {
		Debug.Log($"Player {playerID} landed on space {boardIndex}!");
		// if space ended moving on is a shoot or ladder
		int ladderIndex = FindLadderAt(boardIndex);
		int snakeIndex = FindSnakeAt(boardIndex);

		if( ladderIndex >= 0 ) {
			var ladder = ladders[ladderIndex];
			Debug.Log($"Player {playerID} is taking a ladder from {ladder.StartIndex} to {ladder.EndIndex}!");
			yield return MoveAlong(playerID, ladder.Path, ladder.TransportTime);
			playerPositions[playerID] = ladder.EndIndex;
		}
		else if( snakeIndex >= 0 ) {
			var snake = snakes[snakeIndex];
			Debug.Log($"Player {playerID} is taking a snake from {snake.StartIndex} to {snake.EndIndex}!");
			yield return MoveAlong(playerID, snake.Path, snake.TransportTime);
			playerPositions[playerID] = snake.EndIndex;
		}
	}

	private IEnumerator MoveAlong(int playerID, IReadOnlyPath<Vector3> path, float time) {
		Debug.Log($"Transporter length: {path.Count}");
		Debug.Log($"Total distance of snake: {path.Length}");
		var gamePiece = gamePieces[playerID];
		for( int i = 0; i < path.Count - 1; i++ ) {
			float distanceToTravel = path.GetDistanceBetween(i, i + 1);
			float timeToMove = (distanceToTravel / path.Length) * time;
			/*
			Debug.Log($"i={i} ({transporter.Path[i]}), i+1={i+1} ({transporter.Path[i+1]})");
			Debug.Log($"distance={distanceToTravel}");
			*/
			yield return MoveBetween(gamePiece.transform, path[i], path[i + 1], timeToMove);
		}
		yield return null;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		int index = 0;
		foreach( var cell in GetCellPositions() )
			DrawSpace(cell, index++);
		for( int i = 0; i < snakes.Count; ++i )
			DrawSnake(i);
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
	private Color gizmoSnakeSecondaryColor = Color.yellow;

	private void DrawSnake(int index) {
		var oldColor = Gizmos.color;
		Gizmos.color = gizmoSnakePrimaryColor;

		var snake = snakes[index];
		var start = GetWorldPosition(snake.StartIndex);
		var end = GetWorldPosition(snake.EndIndex);

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
#endif
}