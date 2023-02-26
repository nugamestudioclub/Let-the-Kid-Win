using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Board : MonoBehaviour {
	private const int WIDTH = 7;
	private const int HEIGHT = 7;

	//Snakes
	[SerializeField]
	private List<Transporter> snakes = new();
	private List<int> SnakeStartPositions {
		get => snakes.Select(s => s.StartIndex).ToList();
	}
	//Ladders
	[SerializeField]
	private List<Transporter> ladders = new();
	private List<int> LadderStartPositions {
		get => ladders.Select(l => l.StartIndex).ToList();
	}

	//Game Pieces
	[SerializeField]
	private List<GameObject> gamePieces = new();

	private List<int> playerPositions = new();

	[SerializeField]
	private float moveTime =.5f;
	[SerializeField]
	private float moveSteps = 30f;

	//Spaces
	[SerializeField]
	private List<BoardSpace> spaces = new();

	//Grid
	[SerializeField]
	private Grid grid;

	[SerializeField]
	private BoardSpace spacePrefab;

	void Awake() {
		spaces.AddRange(CreateAllSpaces());
		//initialize starting positions
		foreach( GameObject piece in gamePieces ) {
			playerPositions.Add(0);
			piece.transform.position = spaces[0].transform.position;
		}
	}

	void Update() {
		if( Input.GetKeyDown(KeyCode.Alpha1) ) {
			MovePlayer(0, 1);
		}
		if( Input.GetKeyDown(KeyCode.Alpha2) ) {
			MovePlayer(0, 2);
		}
	}

	public BoardSpace GetSpace(int x, int y) {
		return spaces[IndexOfCell(x, y)];
	}

	public BoardSpace GetSpace(int index) {
		return spaces[index];
	}

	private int IndexOfCell(int x, int y) {
		return (HEIGHT * y) + (y % 2 == 0 ? x : WIDTH - x - 1);
	}

	private IEnumerable<Vector3Int> GetCells() {
		for( int row = 0; row < HEIGHT; ++row )
			if( row % 2 == 0 )
				for( int col = 0; col < WIDTH; ++col )
					yield return new(col, row, 0);
			else
				for( int col = WIDTH - 1; col >= 0; --col )
					yield return new(col, row, 0);
	}

	private List<BoardSpace> CreateAllSpaces() {
		return GetCells().Select(cell => CreateSpace(cell.x, cell.y)).ToList();
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
			yield return StartCoroutine(NextSpace(playerID));
		}
		yield return FinishMoving(playerID, playerPositions[playerID]);
	}

	private IEnumerator NextSpace(int playerID) {
		var currentSpace = spaces[playerPositions[playerID]];
		var nextSpace = spaces[playerPositions[playerID] + 1];

		yield return MovePieceBetweenPoints(playerID, currentSpace.transform.position, nextSpace.transform.position, moveTime);
		playerPositions[playerID]++;
	}

	private IEnumerator MovePieceBetweenPoints(int playerID, Vector2 startPoint, Vector2 endPoint, float moveTime) {
		float moveStep = moveTime / moveSteps;
		for( float i = 0; i < (moveTime + moveStep); i += moveStep ) {
			//take current position and 
			gamePieces[playerID].transform.position = Vector2.Lerp(startPoint, endPoint, i / moveTime);
			yield return new WaitForSeconds(moveStep);
		}
	}

	private IEnumerator FinishMoving(int playerID, int space) {
		Debug.Log($"Player {playerID} landed on space {space}!");
		// if space ended moving on is a shoot or ladder
		int ladderIndex = LadderStartPositions.IndexOf(space);
		int snakeIndex = SnakeStartPositions.IndexOf(space);
		if( ladderIndex != -1 ) {
			var ladder = ladders[ladderIndex];
			Debug.Log($"Player {playerID} is taking a ladder from {ladder.StartIndex} to {ladder.EndIndex}!");
			yield return StartCoroutine(MoveThroughTransporter(
				playerID,
				ladder));
		}
		else if( snakeIndex != -1 ) {
			var snake = snakes[snakeIndex];
			Debug.Log($"Player {playerID} is taking a snake from {snake.StartIndex} to {snake.EndIndex}!");
            yield return StartCoroutine(MoveThroughTransporter(
				playerID,
				snake));
		}
		GameState.Instance.NextState();
	}

	private IEnumerator MoveThroughTransporter(int playerID, Transporter transporter) {
		Debug.Log($"Transporter length: {transporter.Points.Count}");
		Debug.Log($"Total distance of snake: {transporter.TotalLength}");
		for( int i = 0; i < transporter.Points.Count -1; i++ ) {
			float distanceToTravel = transporter.DistanceToNextPoint(i);
			float timeToMove = (distanceToTravel / transporter.TotalLength) * transporter.TransportTime;
			yield return MovePieceBetweenPoints(
				playerID,
				transporter.Points[i],
				transporter.Points[i + 1],
				timeToMove);
		}
		playerPositions[playerID] = transporter.EndIndex;
		yield return null;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		int index = 0;
		foreach( var cell in GetCells() )
			DrawSpace(cell, index++);
	}

	private void DrawSpace(Vector3Int cell, int index) {

		var fill = new Color(1f, 1f, 1f, 0.1f);
		var outline = Color.white;
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
		textStyle.normal.textColor = Color.white;
		Handles.Label(position, index.ToString(), textStyle);
	}
#endif
}