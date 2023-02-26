using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Board : MonoBehaviour
{
    //Snakes
    [SerializeField]
    private List<Transporter> snakes = new();
    private List<int> SnakeStartPositions { 
        get => snakes.Select(s => s.StartIndex).ToList(); 
    }
    //Ladders
    [SerializeField]
    private List<Transporter> ladders = new();
    private List<int> LadderStartPositions
    {
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
    private List<Transform> spaces = new();

    //grid
    [SerializeField]
    private Grid grid;
    void Awake()
    {
        spaces.AddRange(GetGridTransforms());
        //initialize starting positions
        foreach (GameObject piece in gamePieces) {
            playerPositions.Add(0);
            piece.transform.position = spaces[0].position;
        }
    }

    private List<Transform> GetGridTransforms()
    {
        List<Transform> result = new();
        int width = 7;
        int height = 7;
        for (int row = 0; row < height; row++)
        {
            if (row % 2 == 0)
            {
                for (int col = 0; col < width; col++)
                {
                    result.Add(CreateSpace(col, row).transform);
                }
            }
            else
            {
                for (int col = width - 1; col >= 0; col--)
                {
                    result.Add(CreateSpace(col, row).transform);
                }
            }


        }
        return result;
    }

    private GameObject CreateSpace(int x, int y)
    {
        Vector3 spacePosition = grid.CellToWorld(new Vector3Int(x, y, 0));
        GameObject spaceObject = new();
        spaceObject.transform.position = spacePosition;
        return spaceObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MovePlayer(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MovePlayer(0, 2);
        }
    }

    public void MovePlayer(int playerID, int spaces)
    {
        if (gamePieces[playerID] != null)
        {
            //run movement coroutine
            StartCoroutine(GoMovePlayer(playerID, spaces));
        } 
        else
        {
            Debug.Log($"Player {playerID} is not initialized");
        }
    }

    private IEnumerator GoMovePlayer(int playerID, int spaces)
    {
        for (int i = 0; i < spaces; i++)
        {
            yield return StartCoroutine(NextSpace(playerID));
        }
        FinishMoving(playerID, playerPositions[playerID]);
        yield return null;
    }

    private IEnumerator NextSpace(int playerID)
    {
        var currentSpace = spaces[playerPositions[playerID]].position;
        var nextSpace = spaces[playerPositions[playerID] + 1].position;

        yield return MovePieceBetweenPoints(playerID, currentSpace, nextSpace, moveTime);
        playerPositions[playerID]++; 
    }

    private IEnumerator MovePieceBetweenPoints(int playerID, Vector2 startPoint, Vector2 endPoint, float moveTime)
    {
        float moveStep = moveTime / moveSteps;
        for (float i = 0; i < (moveTime + moveStep); i += moveStep)
        {
            //take current position and 
            gamePieces[playerID].transform.position = Vector2.Lerp(startPoint, endPoint, i / moveTime);
            yield return new WaitForSeconds(moveStep);
        }
    }

    private void FinishMoving(int playerID, int space)
    {
        Debug.Log($"Player {playerID} landed on space {space}!");
        // if space ended moving on is a shoot or ladder
        int ladderIndex = LadderStartPositions.IndexOf(space);
        int snakeIndex = SnakeStartPositions.IndexOf(space);
        if (ladderIndex != -1)
        {
            var ladder = ladders[ladderIndex];
            Debug.Log($"Player {playerID} is taking a ladder from {ladder.StartIndex} to {ladder.EndIndex}!");
            StartCoroutine(MoveThroughTransporter(
                playerID,
                ladder));
        }
        else if (snakeIndex != -1)
        {
            var snake = snakes[snakeIndex];
            Debug.Log($"Player {playerID} is taking a snake from {snake.StartIndex} to {snake.EndIndex}!");
            StartCoroutine(MoveThroughTransporter(
                playerID,
                snake));
        }
    }

    private IEnumerator MoveThroughTransporter(int playerID, Transporter transporter)
    {
        Debug.Log($"Transporter length: {transporter.Points.Count}");
        
        for (int i = 0; i < transporter.Points.Count -1; i++) {
            float distanceToTravel = transporter.DistanceToNextPoint(i);
            float timeToMove = (distanceToTravel / transporter.TotalLength) * transporter.TransportTime;
            yield return MovePieceBetweenPoints(
                playerID, 
                transporter.Points[i].position, 
                transporter.Points[i + 1].position, 
                timeToMove);
        }
        playerPositions[playerID] = transporter.EndIndex;
        yield return null;
    }

}
