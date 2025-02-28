using UnityEngine;
using System.Collections;
using TMPro;

//Scripts for the regular goblin enemy
public class Enemy : MonoBehaviour
{
    public Vector2Int currentPosition; //Current position of enemy
    private Vector2Int[] patrolPath; //Patrol path of each enemy
    private int currentPatrolIndex = 0; //Index of the patrol path
    public GameObject arrowLocation; //Spawn location for next move arrow
    public GameObject arrowPrefab; //Prefab for the next move arrow
    private Animator animator; //Animator for the enemy

    public AudioClip attackSound; //Assigned to attacking audio clip in inspector, plays on attack
    public AudioClip footstepSound; //Assigned to footsetp audio clip in inspector, plays on movement
    private float audioVolume = .3f; // Audio volume, 0-1f.
    private float pitchAdjustment;
    private float pitchUpper = .9f;
    private float pitchLower = .7f;
    private float clipDelay = 0f;



    //Gets animator component for the enemy for animations
    private void Start()
    {
        animator = GetComponent<Animator>();
        pitchAdjustment = Random.Range(pitchLower, pitchUpper);
    }
    //Initializes the start position and arrow for the enemy
    public void Initialize(Vector2Int startPosition, Vector2Int[] path)
    {
        currentPosition = startPosition;
        patrolPath = path;
        MoveTowardsTarget();
        PointToNextMove();
    }
    //Performs the turn for the enemy
    public void PerformTurn()
    {
        if (patrolPath != null && patrolPath.Length > 0)
        {
            //Sets to next patrol point and makes footsteps
            Vector2Int targetPosition = patrolPath[currentPatrolIndex % patrolPath.Length];
            MoveEnemy(targetPosition);
            PlayAudioDelayed(footstepSound, clipDelay);

            currentPatrolIndex++;

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
        //Moves towards until it reaches new position
        while (transform.position != new Vector3(currentPosition.x,1f,currentPosition.y))
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(currentPosition.x, 1f, currentPosition.y), 15f * Time.deltaTime);
            yield return null;
        }
        //Once reached, sets animator to not move
        animator.SetBool("IsMoving", false); 
        PointToNextMove();
    }

    //Points arrow to next move location
    public void PointToNextMove()
    {
        if (patrolPath != null && patrolPath.Length > 0)
        {
            Vector2Int targetPosition = patrolPath[currentPatrolIndex];
            //Deletes old arrow
            if (arrowLocation != null)
            {
                Destroy(arrowLocation);
            }
            //Spawns new one in the direction of the next patrol point
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
}
