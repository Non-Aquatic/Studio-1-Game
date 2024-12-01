using UnityEngine;
using System.Collections;

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

    public void Initialize(Vector2Int startPosition, Vector2Int[] path)
    {
        currentPosition = startPosition; 
        patrolPath = path;
        UpdateEnemyPosition();
        PointToNextMove();
    }

    public void PerformTurn()
    {
        if (patrolPath != null && patrolPath.Length > 0)
        {
            Vector2Int targetPosition = patrolPath[currentPatrolIndex];
            MoveTo(targetPosition);
            PlayAudio(footstepSound, footstepSound.length + .25f);

            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPath.Length)
            {
                currentPatrolIndex = 0;
            }
        }
        PointToNextMove();
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

    private void PlayAudio(AudioClip audioInput)
    {
        if(this.TryGetComponent(out AudioSource temp))
        {
            temp.PlayOneShot(audioInput,audioVolume);
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
}
