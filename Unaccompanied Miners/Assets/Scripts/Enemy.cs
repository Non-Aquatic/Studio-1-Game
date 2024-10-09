using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector2Int currentPosition;
    private Vector2Int[] patrolPath;
    private int currentPatrolIndex = 0;
    public AudioClip attackSound; //Assigned to attacking audio clip in inspector, plays on attack
    public AudioClip footstepSound; //Assigned to footsetp audio clip in inspector, plays on movement
    public float audioVolume = .5f; // Audio volume, 0-1f.

    public void Initialize(Vector2Int startPosition, Vector2Int[] path)
    {
        currentPosition = startPosition; 
        patrolPath = path;
        UpdateEnemyPosition();
    }

    public void PerformTurn()
    {
        if (patrolPath != null && patrolPath.Length > 0)
        {
            Vector2Int targetPosition = patrolPath[currentPatrolIndex];
            MoveTo(targetPosition);
            PlayAudio(footstepSound);

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
        transform.position = new Vector3(currentPosition.x, 0.5f, currentPosition.y);
    }

    private void PlayAudio(AudioClip audioInput)
    {
        if(this.TryGetComponent(out AudioSource temp))
        {
            temp.PlayOneShot(audioInput,audioVolume);
        }

    }

}
