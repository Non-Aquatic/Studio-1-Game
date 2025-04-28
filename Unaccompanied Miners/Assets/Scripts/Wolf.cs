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
    public bool finished = true;
    private Animator animator; //Animator for the enemy;

    private Vector2Int targetPosition;
    private List<Vector3> pathToMove = new List<Vector3>(); 
    private int restTurns = 0;
    private Vector2Int nextPosition;
    private enum WolfState { Stall, Move, Rest }
    private WolfState currentState = WolfState.Stall;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
            float distanceToPlayer = float.MaxValue;
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
                    distanceToPlayer = Vector2.Distance(new Vector2(potentialMove.x, potentialMove.y), player.currentPosition);

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
        finished = true;
        currentState = WolfState.Move;
    }

    private void ShowPath(List<Vector3> path)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 position = path[i];


            GameObject scent = Instantiate(scentTrailPrefab, new Vector3(position.x, 0.5f, position.z), Quaternion.identity, transform);

            if (i == path.Count - 1) 
            {
                Vector3 direction = path[i] - path[i - 1];
                scent.transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                Vector3 direction = path[i + 1] - path[i]; 
                scent.transform.rotation = Quaternion.LookRotation(direction);
            }
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
        animator.SetBool("IsMoving", true);
        foreach (var moves in pathToMove)
        {
            Vector3 direction = moves - transform.position;
            int moveX = Mathf.Abs(direction.x) > 0.1f ? (int)Mathf.Sign(direction.x) : 0;
            int moveY = Mathf.Abs(direction.z) > 0.1f ? (int)Mathf.Sign(direction.z) : 0;
            float distance = direction.magnitude;
            direction.Normalize();
            bool notAttacked = true;
            animator.SetInteger("MoveX", moveX);
            animator.SetInteger("MoveY", moveY);
            while (transform.position != moves)
            {
                transform.position = Vector3.MoveTowards(transform.position, moves, moveSpeed*Time.deltaTime*5);
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
        finished = true;
        if (currentState == WolfState.Move || currentState == WolfState.Stall)
        {
            currentState = WolfState.Stall;
        }
        animator.SetBool("IsMoving", false);
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
        finished = true;
    }
    public void PerformAttack()
    {
        player.GetComponent<Player>().TakeDamage(2);
        animator.SetBool("IsAttacking", true);
        restTurns = 4;
        currentState = WolfState.Rest;
        Debug.Log("Yellow");
        StartCoroutine(StopAttackAnimation());
    }
    private IEnumerator StopAttackAnimation()
    {
        yield return new WaitForSeconds(.25f);
        animator.SetBool("IsAttacking", false);
    }
}
