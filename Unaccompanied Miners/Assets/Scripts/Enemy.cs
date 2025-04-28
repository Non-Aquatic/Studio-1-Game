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
    private Vector2Int previousPatrolPosition;
    public bool finished = true;

    public GameObject arrowLocation; //Spawn location for next move arrow
    public GameObject arrowPrefab; //Prefab for the next move arrow
    public GameObject alert;
    private Animator animator; //Animator for the enemy

    public AudioClip attackSound; //Assigned to attacking audio clip in inspector, plays on attack
    public AudioClip footstepSound; //Assigned to footsetp audio clip in inspector, plays on movement
    private float audioVolume = .3f; // Audio volume, 0-1f.
    private float pitchAdjustment;
    private float pitchUpper = .9f;
    private float pitchLower = .7f;
    private float clipDelay = 0f;

    public Player player; 
    public BoardManager boardManager;

    private enum GoblinState { Patrol, Chase, Return }
    private GoblinState currentState = GoblinState.Patrol;

    private int chaseTurnsRemaining = 3;
    private int returnTurnsRemaining = 3;

    private List<Vector2Int> chasePath = new List<Vector2Int>();

    //Gets animator component for the enemy for animations
    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        boardManager = FindObjectOfType<BoardManager>();
        pitchAdjustment = Random.Range(pitchLower, pitchUpper);
        alert.SetActive(false);
    }
    //Initializes the start position and arrow for the enemy
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
    public void Patrol()
    {
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
    private void ChasePlayer()
    {
        if (chaseTurnsRemaining > 0)
        {
            Vector2Int targetPosition = player.currentPosition;

            Vector3 closestMove = Vector3.zero;
            float closestDistance = float.MaxValue;

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

            if (closestMove != Vector3.zero)
            {
                chasePath.Add(currentPosition); 
                MoveEnemy(new Vector2Int((int)closestMove.x, (int)closestMove.z));
            }

            PlayAudioDelayed(footstepSound, clipDelay);

            chaseTurnsRemaining--;
        }
    }
    private void ReturnToPatrol()
    {
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
        int oneAhead = (currentPatrolIndex) % patrolPath.Length;
        int twoAhead = (currentPatrolIndex + 1) % patrolPath.Length;

        Vector2Int nextTile = patrolPath[oneAhead];
        Vector2Int nextNextTile = patrolPath[twoAhead];

        if (nextTile == player.currentPosition || nextNextTile == player.currentPosition & currentState == GoblinState.Patrol)
        {
            alert.SetActive(true);
            StartCoroutine(AlertEnding(1f)); 
            currentState = GoblinState.Chase;
            chaseTurnsRemaining = 3;
            chasePath.Clear();
            previousPatrolPosition = currentPosition;
        }
        if (currentState == GoblinState.Chase && chaseTurnsRemaining <= 0)
        {
            currentState = GoblinState.Return;
            returnTurnsRemaining = 3;
        }
        if (currentState == GoblinState.Return && returnTurnsRemaining <= 0)
        {
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
            case GoblinState.Patrol:
                if (patrolPath != null && patrolPath.Length > 0)
                {
                    targetPosition = patrolPath[currentPatrolIndex % patrolPath.Length];
                }
                break;

            case GoblinState.Chase:
                targetPosition = ChasebutDontMove().Value;
                break;

            case GoblinState.Return:
                targetPosition = chasePath[chasePath.Count - 1];
                break;
        }

        if (targetPosition != Vector2Int.zero)
        {
            if (arrowLocation != null)
            {
                Destroy(arrowLocation); 
            }
            arrowLocation = Instantiate(arrowPrefab, transform.position + Vector3.up, Quaternion.identity);
            Vector3 direction = new Vector3(targetPosition.x - currentPosition.x, 0f, targetPosition.y - currentPosition.y).normalized;
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
    private Vector2Int? ChasebutDontMove()
    {
        Vector2Int targetPosition = player.currentPosition;
        float closestDistance = float.MaxValue;
        Vector2Int? bestMove = null;

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
    private IEnumerator AlertEnding(float delay)
    {
        yield return new WaitForSeconds(delay);
        alert.SetActive(false); 
    }
}
