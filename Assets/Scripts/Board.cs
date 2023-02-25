using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    void Awake()
    {
        //initialize starting positions
        foreach (GameObject piece in gamePieces) {
            playerPositions.Add(0);
            piece.transform.position = spaces[0].position;
        }
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
            FinishMoving(playerID, playerPositions[playerID]);
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
            //yield return new WaitForSeconds(moveTime);
        }
        yield return null;
    }

    private IEnumerator NextSpace(int playerID)
    {
        //lerp between positions
        float moveStep = moveTime / moveSteps;
        var currentSpace = spaces[playerPositions[playerID]].position;
        var nextSpace = spaces[playerPositions[playerID] + 1].position;
        for (float i = 0; i < (moveTime + moveStep); i += moveStep)
        {
            //take current position and 
            gamePieces[playerID].transform.position = Vector2.Lerp(currentSpace, nextSpace, i/moveTime);
            yield return new WaitForSeconds(moveStep);
        }
        playerPositions[playerID]++; 
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
            StartCoroutine(MoveThroughPoints(
                playerID, 
                ladder.Points, 
                ladder.TotalLength(), 
                ladder.TransportTime));
        }
        else if (snakeIndex != -1)
        {
            var snake = snakes[snakeIndex];
            StartCoroutine(MoveThroughPoints(
                playerID, 
                snake.Points, 
                snake.TotalLength(), 
                snake.TransportTime));
        }
    }

    private IEnumerator MoveThroughPoints(int playerID, List<Transform> points, float totalDistance, float time)
    {
        yield return null;
    }

}
