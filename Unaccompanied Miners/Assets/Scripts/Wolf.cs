using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf : MonoBehaviour
{
    public float moveSpeed = 10f;
    public GameObject scentTrailPrefab; 
    public Player player; 
    public Vector2Int currentPosition;
    public BoardManager boardManager;


    private Vector2Int targetPosition;
    private List<Vector3> pathToMove = new List<Vector3>(); 
    private int restTurns = 0;
    private Vector2Int nextPosition;
    private enum WolfState { Stall, Move, Rest }
    private WolfState currentState = WolfState.Stall;

    private void Start()
    {
        player = FindObjectOfType<Player>(); 
        boardManager = FindObjectOfType<BoardManager>();
        targetPosition = player.currentPosition;
    }

    public void Initialize(Vector2Int startPosition)
    {
        currentPosition = startPosition;
        nextPosition = startPosition;
        transform.position = new Vector3(currentPosition.x, 1f, currentPosition.y);
    }
    public void UpdateTargetPosition()
    {
        targetPosition = player.currentPosition;
    }

    public void PerformTurn()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case WolfState.Stall:
                Stall();
                break;
            case WolfState.Move:
                Move();
                break;
            case WolfState.Rest:
                Rest();
                break;
        }
    }

    private void Stall()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 currentMove = Vector3.zero;
            float close = float.MaxValue;
            List<Vector2Int> directions = new List<Vector2Int>()
                {
                    new Vector2Int(nextPosition.x + 1, nextPosition.y),
                    new Vector2Int(nextPosition.x - 1, nextPosition.y),
                    new Vector2Int(nextPosition.x, nextPosition.y + 1),
                    new Vector2Int(nextPosition.x, nextPosition.y - 1),
                };

            foreach (var potentialMove in directions)
            {
                if (boardManager.IsTileTraversable(potentialMove) && !pathToMove.Contains(new Vector3(potentialMove.x, 1f, potentialMove.y)))
                {
                    float distanceToPlayer = Vector2.Distance(new Vector2(potentialMove.x, potentialMove.y), targetPosition);

                    if (distanceToPlayer < close)
                    {
                        close = distanceToPlayer;
                        currentMove = new Vector3(potentialMove.x, 1f, potentialMove.y);
                    }
                }
            }

            if (currentMove != Vector3.zero)
            {
                pathToMove.Add(currentMove);
                nextPosition = new Vector2Int((int)currentMove.x,(int)(currentMove.z));
            }
        }
    
        ShowPath(pathToMove);
        currentState = WolfState.Move;
    }

    private void ShowPath(List<Vector3> path)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var position in path)
        {
            Instantiate(scentTrailPrefab, new Vector3(position.x, 0.1f, position.z), Quaternion.identity, transform);
        }
    }

    private void Move()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (pathToMove.Count > 0)
        {
            StartCoroutine(MoveThroughPath());
        }
    }

    private IEnumerator MoveThroughPath()
    {
        foreach (var moves in pathToMove)
        {
            bool notAttacked = true;
            while (transform.position != moves)
            {
                transform.position = Vector3.MoveTowards(transform.position, moves, moveSpeed*Time.deltaTime);
                currentPosition = new Vector2Int((int)moves.x, (int)moves.z);
                if (currentPosition == player.currentPosition && notAttacked)
                {
                    PerformAttack();
                    notAttacked = false;
                }
                yield return null; 
            } 
        }
        pathToMove.Clear();
        if(currentState == WolfState.Move || currentState == WolfState.Stall)
        {
            currentState = WolfState.Stall;
        }
    }


    private void Rest()
    {
        if (restTurns > 0)
        {
            restTurns--;
        }
        else
        {
            currentState = WolfState.Stall;
        }
    }
    public void PerformAttack()
    {
        player.GetComponent<Player>().TakeDamage(2);
        restTurns = 4;
        currentState = WolfState.Rest;
    }
}
