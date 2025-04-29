using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

//Scripts for the regular goblin enemy
public class Enemy : MonoBehaviour
{
    public Vector2Int currentPosition; //Current position of enemy
    private Vector2Int[] patrolPath; //Patrol path of each enemy
    private int currentPatrolIndex = 0; //Index of the patrol path
    private Vector2Int previousPatrolPosition; //Previous Patrol position of the enemy
    public bool finished = true; 

    public GameObject arrowLocation; //Spawn location for next move arrow
    public GameObject arrowPrefab; //Prefab for the next move arrow
    public GameObject alert; //Alert gameobject for the chase state
    private Animator animator; //Animator for the enemy

    public AudioClip attackSound; //Assigned to attacking audio clip in inspector, plays on attack
    public AudioClip footstepSound; //Assigned to footsetp audio clip in inspector, plays on movement
    private float audioVolume = .3f; // Audio volume, 0-1f.
    private float pitchAdjustment; //Randomized pitch adjustment for goblin sounds
    private float pitchUpper = .9f; //Higher end of pitch adjustments
    private float pitchLower = .7f; //Lower end of pitch adjustments
    private float clipDelay = 0f; //Delay amount for the goblin sounds

    public Player player;  //Reference to the player
    public BoardManager boardManager; //Reference to the board

    private enum GoblinState { Patrol, Chase, Return } //Different possible states of the goblin
    private GoblinState currentState = GoblinState.Patrol; //Current state of the goblin

    private int chaseTurnsRemaining = 3; //Current amount of chase turns remaining, max is 3
    private int returnTurnsRemaining = 3;//Current amount of return turns remaining, max is 3

    private List<Vector2Int> chasePath = new List<Vector2Int>(); //Current chase path of the goblin

    private void Start()
    {
        //Gets animator component for the enemy for animations
        animator = GetComponent<Animator>();
        //Finds the reference to the player in the scene
        player = FindObjectOfType<Player>();
        //Finds the reference to the board manager in the scene
        boardManager = FindObjectOfType<BoardManager>();
        //Randomly chooses a pitch adjustment between the min and max
        pitchAdjustment = Random.Range(pitchLower, pitchUpper);
        //Deactivates the alert object
        alert.SetActive(false);
    }

    //Initializes the start position, pathing, and arrow for the enemy
    public void Initialize(Vector2Int startPosition, Vector2Int[] path)
    {
        currentPosition = startPosition;
        patrolPath = path;
        previousPatrolPosition = patrolPath[currentPatrolIndex];
        MoveTowardsTarget();
        PointToNextMove();
    }

    //Performs the turn for the enemy
    public void PerformTurn()
    {
        //Chooses the action to perform based on the state
        switch (currentState)
        {
            case GoblinState.Patrol:
                Patrol();
                break;

            case GoblinState.Chase:
                ChasePlayer();
                break;

            case GoblinState.Return:
                ReturnToPatrol();
                break;
        }
    }
    //Enemy patrols through predetermined positions
    public void Patrol()
    {
        //Chooses the next patrol positon as its next movement location and cycles to the next one
        if (patrolPath != null && patrolPath.Length > 0)
        {
            //Sets to next patrol point and makes footsteps
            //Debug.Log($"{this.name}: Going to position {currentPatrolIndex % patrolPath.Length}");
            Vector2Int targetPosition = patrolPath[currentPatrolIndex % patrolPath.Length];
            MoveEnemy(targetPosition);
            PlayAudioDelayed(footstepSound, clipDelay);

            currentPatrolIndex++;

        }

    }
    //Enemey chases the player
    private void ChasePlayer()
    {
        //If the goblin has not done three turns of chasing
        if (chaseTurnsRemaining > 0)
        {
            Vector2Int targetPosition = player.currentPosition;

            Vector3 closestMove = Vector3.zero;
            float closestDistance = float.MaxValue;

            //Chooses from all four directions as sees which one is closest to the player, a valid location, and has not already been traveled on 
            List<Vector2Int> directions = new List<Vector2Int>()
            {
                new Vector2Int(currentPosition.x + 1, currentPosition.y),
                new Vector2Int(currentPosition.x - 1, currentPosition.y),
                new Vector2Int(currentPosition.x, currentPosition.y + 1),
                new Vector2Int(currentPosition.x, currentPosition.y - 1),
            };

            foreach (var potentialMove in directions)
            {
                if (boardManager.IsTileTraversable(potentialMove) && !chasePath.Contains(potentialMove))
                {
                    float distanceToPlayer = Vector2.Distance(new Vector2(potentialMove.x, potentialMove.y), targetPosition);

                    if (distanceToPlayer < closestDistance)
                    {
                        closestDistance = distanceToPlayer;
                        closestMove = new Vector3(potentialMove.x, 1f, potentialMove.y);
                    }
                }
            }
            //It chooses that space and adds it to the chase path adn sets it as its next movement location
            if (closestMove != Vector3.zero)
            {
                chasePath.Add(currentPosition); 
                MoveEnemy(new Vector2Int((int)closestMove.x, (int)closestMove.z));
            }

            PlayAudioDelayed(footstepSound, clipDelay);

            chaseTurnsRemaining--;
        }
    }
    //Enemey returns back to patrolling
    private void ReturnToPatrol()
    {
        //Goes bacl through the inverse of the chase path step by step untill it returns to where it was before
        if (returnTurnsRemaining > 0 && chasePath.Count > 0)
        {
            Vector2Int returnStep = chasePath[chasePath.Count - 1];
            chasePath.RemoveAt(chasePath.Count - 1);
            MoveEnemy(returnStep);

            PlayAudioDelayed(footstepSound, clipDelay);

            returnTurnsRemaining--;
        }
    }

    //Sets the new position for the enemy
    private void MoveEnemy(Vector2Int newPosition)
    {
        Vector2Int direction = newPosition - currentPosition;
        currentPosition = newPosition;
        //Sets the animators to play walking animation
        animator.SetBool("IsMoving", true);
        animator.SetInteger("MoveX", direction.x); 
        animator.SetInteger("MoveY", direction.y); 
        StartCoroutine(MoveTowardsTarget());
    }
    //Moves enemy to new location smoothly
    private IEnumerator MoveTowardsTarget()
    {
        //Deletes old arrow
        if (arrowLocation != null)
        {
            Destroy(arrowLocation);
        }
        //Moves enemy using lerp until it gets extremely close to the desired position
        Vector3 newPosition = new Vector3(currentPosition.x, 1f, currentPosition.y);
        while (Vector3.Distance(transform.position, newPosition) > 0.0001f)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, 15f * Time.deltaTime);
            yield return null;
        }

        //Moves towards until it reaches new position
       /* while (transform.position != new Vector3(currentPosition.x,1f,currentPosition.y))
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(currentPosition.x, 1f, currentPosition.y), 15f * Time.deltaTime);
            yield return null;
        }*/
        //Once reached, sets animator to not move
        animator.SetBool("IsMoving", false);
        finished = true;
        //Sets the new positions that are two infront of the goblin
        int oneAhead = (currentPatrolIndex) % patrolPath.Length;
        int twoAhead = (currentPatrolIndex + 1) % patrolPath.Length;

        Vector2Int nextTile = patrolPath[oneAhead];
        Vector2Int nextNextTile = patrolPath[twoAhead];

        //Changes states depending on whether the goblin meets the criteria
        //Goes from Patrol to Chase if the player is within two spaces of the goblin
        if (nextTile == player.currentPosition || nextNextTile == player.currentPosition & currentState == GoblinState.Patrol)
        {
            //Activates alert to warn player that the goblin is chasing the player
            alert.SetActive(true);
            StartCoroutine(AlertEnding(1f)); 
            //Changes state to chase
            currentState = GoblinState.Chase;
            chaseTurnsRemaining = 3;
            chasePath.Clear();
            previousPatrolPosition = currentPosition;
        }
        //Goes from Chase to Return if the goblin has chased for 3 turns
        if (currentState == GoblinState.Chase && chaseTurnsRemaining <= 0)
        {
            //Changes state to return
            currentState = GoblinState.Return;
            returnTurnsRemaining = 3;
        }
        //Goes from Return to Patrol if the goblin has returned back to its original spot 
        if (currentState == GoblinState.Return && returnTurnsRemaining <= 0)
        {
            //Changes state to return
            currentState = GoblinState.Patrol;
        }
        PointToNextMove();
    }

    //Points arrow to next move location
    public void PointToNextMove()
    {
        Vector2Int targetPosition = Vector2Int.zero;
   
        switch (currentState)
        {
            //If in patrol it points to next patrol point
            case GoblinState.Patrol:
                if (patrolPath != null && patrolPath.Length > 0)
                {
                    targetPosition = patrolPath[currentPatrolIndex % patrolPath.Length];
                }
                break;
            //If in chase it points to next position nearest to the player
            case GoblinState.Chase:
                targetPosition = ChasebutDontMove();
                break;
            //If in return it points to next position in the inverse of the chase path
            case GoblinState.Return:
                targetPosition = chasePath[chasePath.Count - 1];
                break;
        }

        if (targetPosition != Vector2Int.zero)
        {
            //Gets rid of old arrow
            if (arrowLocation != null)
            {
                Destroy(arrowLocation); 
            }
            arrowLocation = Instantiate(arrowPrefab, transform.position + Vector3.up, Quaternion.identity);
            Vector3 direction = new Vector3(targetPosition.x - currentPosition.x, 0f, targetPosition.y - currentPosition.y).normalized;
            //Makes sure old arrow points in correct location
            arrowLocation.transform.rotation = Quaternion.LookRotation(direction);
        }
    }


    //Plays audio with a specific delay
    public void PlayAudioDelayed(AudioClip audioInput, float delay)
    {
        if (this.TryGetComponent(out AudioSource temp))
        {
            temp.loop = false;
            temp.clip = audioInput;
            temp.volume = audioVolume;
            temp.pitch = pitchAdjustment;
            temp.PlayDelayed(delay);

        }
    }

    //Sets animator to play attack animation
    public void PerformAttack()
    {
        animator.SetBool("IsAttacking", true);
        StartCoroutine(StopAttackAnimation());
    }
    //Stops attack animation
    private IEnumerator StopAttackAnimation()
    {
        yield return new WaitForSeconds(.25f);
        animator.SetBool("IsAttacking", false);
    }
    //Does the exact same thing as the Chase Player but doen't move the player in order to determine where the arrow points beforehand
    private Vector2Int ChasebutDontMove()
    {
        Vector2Int targetPosition = player.currentPosition;
        float closestDistance = float.MaxValue;
        Vector2Int bestMove = Vector2Int.zero;

        List<Vector2Int> directions = new List<Vector2Int>()
    {
        new Vector2Int(currentPosition.x + 1, currentPosition.y),
        new Vector2Int(currentPosition.x - 1, currentPosition.y),
        new Vector2Int(currentPosition.x, currentPosition.y + 1),
        new Vector2Int(currentPosition.x, currentPosition.y - 1),
    };

        foreach (var potentialMove in directions)
        {
            if (boardManager.IsTileTraversable(potentialMove) && !chasePath.Contains(potentialMove))
            {
                float distanceToPlayer = Vector2.Distance(potentialMove, targetPosition);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    bestMove = potentialMove;
                }
            }
        }

        return bestMove;
    }
    //Waits for a bit before deactivating the alert
    private IEnumerator AlertEnding(float delay)
    {
        yield return new WaitForSeconds(delay);
        alert.SetActive(false); 
    }
}
