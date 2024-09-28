using UnityEngine;

public class Player : MonoBehaviour
{
    public BoardManager boardManager;
    public TurnManager turnManager; 
    public Vector2Int currentPosition;
    public int gemCount; 
    public int health;
    private int miningSuccessChance = 70;

    private void Start()
    {
        currentPosition = new Vector2Int(0, 0); 
        UpdatePlayerPosition();
    }

    private void Update()
    {
        if (enabled)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                AttemptMove(new Vector2Int(0, 1));
            }
            else if (Input.GetKeyDown(KeyCode.S)) 
            {
                AttemptMove(new Vector2Int(0, -1));
            }
            else if (Input.GetKeyDown(KeyCode.A)) 
            {
                AttemptMove(new Vector2Int(-1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.D)) 
            {
                AttemptMove(new Vector2Int(1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.M)) 
            {
                AttemptMining(currentPosition);
            }
        }
    }

    private void AttemptMove(Vector2Int direction)
    {
        Vector2Int targetPosition = currentPosition + direction;

        if (boardManager.IsTileTraversable(targetPosition))
        {
            MoveTo(targetPosition);
            turnManager.EndPlayerTurn(); 
        }
    }

    private void MoveTo(Vector2Int newPosition)
    {
        currentPosition = newPosition;
        UpdatePlayerPosition();
    }

    private void UpdatePlayerPosition()
    {
        transform.position = new Vector3(currentPosition.x, 0.5f, currentPosition.y);
    }
    public void AddGems(int amount)
    {
        gemCount += amount;
        Debug.Log("Gems collected:"+gemCount);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Player is dead.");
        }
    }
    private void AttemptMining(Vector2Int position)
    {
        if (boardManager.IsMiningNode(position))
        {
            int successRoll = Random.Range(0, 100);
            if (successRoll < miningSuccessChance)
            {
                int gemsMined = Random.Range(1, 5); 
                AddGems(gemsMined);
                Debug.Log("Successfully mined " +gemsMined+" gems!");
            }
            else
            {
                Debug.Log("Mining attempt failed.");
            }
            turnManager.EndPlayerTurn(); 
        }
        else
        {
            Debug.Log("Not on a mining node!");
        }
    }
}
