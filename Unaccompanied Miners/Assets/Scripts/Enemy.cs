using UnityEngine;
using System.Collections;
using TMPro;

public class Enemy : MonoBehaviour
{
    public Vector2Int currentPosition;
    private Vector2Int[] patrolPath;
    private int currentPatrolIndex = 0;
    public AudioClip attackSound; //Assigned to attacking audio clip in inspector, plays on attack
    public AudioClip footstepSound; //Assigned to footsetp audio clip in inspector, plays on movement
    public float audioVolume = .3f; // Audio volume, 0-1f.
    public GameObject arrowLocation;
    public GameObject arrowPrefab;
    private Animator animator;
    private Vector3 targetPosition;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Initialize(Vector2Int startPosition, Vector2Int[] path)
    {
        currentPosition = startPosition;
        patrolPath = path;
        MoveTowardsTarget();
        PointToNextMove();
    }

    public void PerformTurn()
    {
        if (patrolPath != null && patrolPath.Length > 0)
        {
            Vector2Int targetPosition = patrolPath[currentPatrolIndex];
            MoveEnemy(targetPosition);
            PlayAudio(footstepSound, footstepSound.length + .25f);

            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPath.Length)
            {
                currentPatrolIndex = 0;
            }
        }

    }

    private void MoveTo(Vector2Int newPosition)
    {
        currentPosition = newPosition;
        UpdateEnemyPosition();
    }

    private void UpdateEnemyPosition()
    {
        transform.position = new Vector3(currentPosition.x, 1f, currentPosition.y);
    }
    
    private void MoveEnemy(Vector2Int newPosition)
    {
        Vector2Int direction = newPosition - currentPosition;
        currentPosition = newPosition;
        animator.SetBool("IsMoving", true);
        animator.SetInteger("MoveX", direction.x); 
        animator.SetInteger("MoveY", direction.y); 
        StartCoroutine(MoveTowardsTarget());
    }

    private IEnumerator MoveTowardsTarget()
    {
        while (transform.position != new Vector3(currentPosition.x,1f,currentPosition.y))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentPosition.x, 1f, currentPosition.y), 5f * Time.deltaTime);
            yield return null; 
        }
        animator.SetBool("IsMoving", false); 
        PointToNextMove();
    }

    private void PlayAudio(AudioClip audioInput)
    {
        if (this.TryGetComponent(out AudioSource temp))
        {
            temp.PlayOneShot(audioInput, audioVolume);
        }

    }

    public void PointToNextMove()
    {
        if (patrolPath != null && patrolPath.Length > 0)
        {
            Vector2Int targetPosition = patrolPath[currentPatrolIndex];
            if (arrowLocation != null)
            {
                Destroy(arrowLocation);
            }
            arrowLocation = Instantiate(arrowPrefab, transform.position + Vector3.up, Quaternion.identity);
            Vector3 direction = new Vector3(targetPosition.x - currentPosition.x, 0f, targetPosition.y - currentPosition.y).normalized;
            arrowLocation.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void PlayAudio(AudioClip audioInput, float delay)
    {
        if (this.TryGetComponent(out AudioSource temp))
        {
            temp.loop = false;
            temp.clip = audioInput;
            temp.volume = audioVolume;
            temp.PlayDelayed(.25f);

        }

    }
    public void PerformAttack()
    {
        animator.SetBool("IsAttacking", true);
        StartCoroutine(StopAttackAnimation());
    }

    private IEnumerator StopAttackAnimation()
    {
        yield return new WaitForSeconds(.25f);
        animator.SetBool("IsAttacking", false);
    }
}
