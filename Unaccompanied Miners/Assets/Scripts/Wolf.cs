using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf : MonoBehaviour
{
    public float moveSpeed = 2f;
    public GameObject scentTrailPrefab; 
    public Transform player; 
    public Vector2Int currentPosition;
    public BoardManager boardManager;


    private Vector3 targetPosition;
    private List<Vector3> pathToMove = new List<Vector3>(); 
    private int restTurns = 0; 
    private enum WolfState { Stall, Move, Rest }
    private WolfState currentState = WolfState.Stall;

    private void Start()
    {
        targetPosition = player.position;
    }

    public void Initialize(Vector2Int startPosition)
    {
        currentPosition = startPosition;
        transform.position = new Vector3(currentPosition.x, 1f, currentPosition.y);
    }
    public void UpdateTargetPosition()
    {
        targetPosition = player.position;
    }

    public void PerformTurn()
    {
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
                    new Vector2Int(currentPosition.x + 1, currentPosition.y),
                    new Vector2Int(currentPosition.x - 1, currentPosition.y),
                    new Vector2Int(currentPosition.x, currentPosition.y + 1),
                    new Vector2Int(currentPosition.x, currentPosition.y - 1),
                };

            foreach (var potentialMove in directions)
            {
                if (boardManager.IsTileTraversable(potentialMove))
                {
                    float distanceToPlayer = Vector2.Distance(new Vector2(potentialMove.x, potentialMove.y), new Vector2(targetPosition.x, targetPosition.z));

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
                currentPosition = new Vector2Int(Mathf.FloorToInt(currentMove.x), Mathf.FloorToInt(currentMove.z));
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
        if (pathToMove.Count > 0)
        {
            for (int i = 0; i < pathToMove.Count; i++)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathToMove[i], moveSpeed * Time.deltaTime);
            }
            pathToMove.Clear();
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
        if (transform.position == player.position)
        {
            player.GetComponent<Player>().TakeDamage(2); 
            restTurns = 4; 
            currentState = WolfState.Rest; 
        }
    }
}
