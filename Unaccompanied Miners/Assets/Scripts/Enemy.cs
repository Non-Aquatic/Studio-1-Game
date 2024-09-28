using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector2Int currentPosition;
    private Vector2Int[] patrolPath;
    private int currentPatrolIndex = 0;

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
}
