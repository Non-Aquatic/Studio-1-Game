using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wolf : MonoBehaviour
{
    public float moveSpeed = 10f; //Movement speed for the wolf
    public GameObject scentTrailPrefab; //Prefab for movement indicators
    public Player player; //Reference to player
    public Vector2Int currentPosition; //Current location of wolf
    public BoardManager boardManager; //Reference to Board Manager
    public bool finished = true; //Bool for whether the wolf has finished performing its action
    private Animator animator; //Animator for the enemy;

    private Vector2Int targetPosition; //Target Position of the wolf, where to move
    private List<Vector3> pathToMove = new List<Vector3>(); //List of the path the wolf will take
    private int restTurns = 0; //Current amount of rest turns left
    private Vector2Int nextPosition; //Next position of the wolf
    private enum WolfState { Stall, Move, Rest } //Possible wolf behavior states
    private WolfState currentState = WolfState.Stall; //Current wolf behavior state

    //Gathers all necessary components and sets target position to the player
    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>(); 
        boardManager = FindObjectOfType<BoardManager>();
        targetPosition = player.currentPosition;
    }
    //Method to intialize the wolf with a spawn position
    public void Initialize(Vector2Int startPosition)
    {
        currentPosition = startPosition;
        nextPosition = startPosition;
        transform.position = new Vector3(currentPosition.x, 1f, currentPosition.y);
    }
    //Updates the wolves target position
    public void UpdateTargetPosition()
    {
        targetPosition = player.currentPosition;
    }
    //Method to allow wolf to perform its turn based on its current behavior state
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
    //Method to make the Wolf stands still and finds three subsequent spaces that will take it towards the player
    private void Stall()
    {
        //Until it finds three spots
        for (int i = 0; i < 3; i++)
        {
            //Will look in all four directions from the new position for a valid move that is closest to the player, is valid traversable tile, and hasn;t already been moved on
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
            //Takes that spot adds it into its path to move and sets its next position to it 
            if (currentMove != Vector3.zero)
            {
                pathToMove.Add(currentMove);
                nextPosition = new Vector2Int((int)currentMove.x,(int)(currentMove.z));
            }
        }
        //Displays the path and changes the behavior to move
        ShowPath(pathToMove);
        finished = true;
        currentState = WolfState.Move;
    }
    //Method to display the path that the wolf will take
    private void ShowPath(List<Vector3> path)
    {
        //Destroys all the previous paths
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        //For every space in the path list, it will instantiate a pathing prefab
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 position = path[i];


            GameObject scent = Instantiate(scentTrailPrefab, new Vector3(position.x, 0.5f, position.z), Quaternion.identity, transform);

            //Makes sure that last on points to the previous one, to make it look nice
            if (i == path.Count - 1) 
            {
                Vector3 direction = path[i] - path[i - 1];
                scent.transform.rotation = Quaternion.LookRotation(direction);
            }
            //All previous ones point to the one in front of them
            else
            {
                Vector3 direction = path[i + 1] - path[i]; 
                scent.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    //Method to move the wolf
    private void Move()
    {
        //Destroys all movement indicators
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        //Traverses through the path using a coroutine
        if (pathToMove.Count > 0)
        {
            StartCoroutine(MoveThroughPath());
        }
    }

    private IEnumerator MoveThroughPath()
    {
        //Sets animator to moving
        animator.SetBool("IsMoving", true);
        //For all spaces in the path
        foreach (var moves in pathToMove)
        {
            //Gets the direction of movement and sets the animator to that direction
            Vector3 direction = moves - transform.position;
            int moveX = Mathf.Abs(direction.x) > 0.1f ? (int)Mathf.Sign(direction.x) : 0;
            int moveY = Mathf.Abs(direction.z) > 0.1f ? (int)Mathf.Sign(direction.z) : 0;
            float distance = direction.magnitude;
            direction.Normalize();
            bool notAttacked = true;
            animator.SetInteger("MoveX", moveX);
            animator.SetInteger("MoveY", moveY);
            //Will move the wolf to the desired location
            while (transform.position != moves)
            {
                transform.position = Vector3.MoveTowards(transform.position, moves, moveSpeed*Time.deltaTime*5);
                currentPosition = new Vector2Int((int)moves.x, (int)moves.z);
                //If the player and wolf are on the same spot the wolf will perform an attack
                if (currentPosition == player.currentPosition && notAttacked)
                {
                    PerformAttack();
                    notAttacked = false;
                }
                yield return null; 
            } 
        }
        //After moving clears the path for next stall cycle 
        pathToMove.Clear();
        finished = true;
        //Changes behavior state to Stall
        if (currentState == WolfState.Move || currentState == WolfState.Stall)
        {
            currentState = WolfState.Stall;
        }
        //Sets animators to not moving
        animator.SetBool("IsMoving", false);
    }

    //Method to make the wolf not move at all after attacking the player
    private void Rest()
    {
        //If the wold still has rest turn then do nothing
        if (restTurns > 0)
        {
            restTurns--;
        }
        //else change behavior back into Stall
        else
        {
            currentState = WolfState.Stall;
        }
        finished = true;
    }
    //Method to perform attack on player
    public void PerformAttack()
    {
        //Makes the player take damage
        player.GetComponent<Player>().TakeDamage(2);
        //Sets animator to attack
        animator.SetBool("IsAttacking", true);
        //Changes behavior to rest
        restTurns = 4;
        currentState = WolfState.Rest;
        StartCoroutine(StopAttackAnimation());
    }
    //Method to wait for attack animation to finish playing before setting attack to false
    private IEnumerator StopAttackAnimation()
    {
        yield return new WaitForSeconds(.25f);
        animator.SetBool("IsAttacking", false);
    }
}
