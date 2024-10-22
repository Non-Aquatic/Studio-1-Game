using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public BoardManager boardManager;
    public TurnManager turnManager; 
    public Vector2Int currentPosition;
    public float moveSpeed = 5f; 
    private Vector3 targetPosition; 
    private Animator animator;
    public int gemCount;
    public int health;
    private int miningSuccessChance = 70;
    public AudioClip miningSound; //Assigned to mining audio clip in inspector, plays on mine
    public AudioClip gemCollect;
    public AudioClip takeDamage;
    public AudioClip footstepSound; //Assigned to footsetp audio clip in inspector, plays on movement
    public float audioVolume = .5f; // Audio volume, 0-1f.

    private void Start()
    {
        health = 10;
        turnManager.UpdateUI();
        currentPosition = new Vector2Int(0, 0);
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        MovePlayer();
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
    private void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            animator.SetBool("IsMoving", false); 
        }
    }
    private void AttemptMove(Vector2Int direction)
    {
        Vector2Int targetPosition = currentPosition + direction;

        if (boardManager.IsTileTraversable(targetPosition))
        {
            MoveTo(targetPosition, direction);
            turnManager.EndPlayerTurn(); 
        }
    }

    private void MoveTo(Vector2Int newPosition, Vector2Int direction)
    {
        currentPosition = newPosition;
        targetPosition = new Vector3(currentPosition.x, 1f, currentPosition.y);
        PlayAudio(footstepSound);
        animator.SetBool("IsMoving", true);

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }
    public void AddGems(int amount)
    {
        gemCount += amount;
        Debug.Log("Gems collected:"+gemCount);
        PlayAudio(gemCollect, miningSound.length + .25f);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        PlayAudio(takeDamage);
    }
    private void AttemptMining(Vector2Int position)
    {
        if (boardManager.IsMiningNode(position))
        {
            PlayAudio(miningSound);
            int successRoll = Random.Range(0, 100);
            if (successRoll < miningSuccessChance)
            {
                int gemsAvailable = boardManager.gemCounts[position.y, position.x]; 
                if (gemsAvailable > 0)
                {
                    int gemsMined = Random.Range(1, 5);
                    AddGems(gemsMined);

                    gemsAvailable -= gemsMined;
                    boardManager.gemCounts[position.y, position.x] = gemsAvailable;

                    if (gemsAvailable <= 0)
                    {
                        boardManager.gridLayout[position.y, position.x] = 1;
                        boardManager.ReplaceTile(position);
                    }
                }
            }
            else
            {
                Debug.Log("Failed to mine gems");
            }
            turnManager.EndPlayerTurn(); 
        }
        else
        {
            Debug.Log("Mine at a node");
        }
    }

    private void PlayAudio(AudioClip audioInput)
    {
        if (this.TryGetComponent(out AudioSource temp))
        {
            temp.PlayOneShot(audioInput, audioVolume);
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
