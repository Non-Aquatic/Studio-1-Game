using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public BoardManager boardManager;
    public TurnManager turnManager; 
    public Vector2Int currentPosition;
    public float moveSpeed = 5f; 
    private Vector3 targetPosition; 
    private Animator animator;
    //public Watchtower watchtower;

    public int gemCount;
    public int health;
    public int maxHealth = 10;
    public HealthBar healthBar;
    private int miningSuccessChance = 80;

    public AudioClip miningSound; //Assigned to mining audio clip in inspector, plays on mine
    public AudioClip gemCollect;
    public AudioClip takeDamage;
    public AudioClip footstepSound; //Assigned to footsetp audio clip in inspector, plays on movement

    public float audioVolume = .5f; // Audio volume, 0-1f.
    private bool isMoving = false;
    public string boardState = "";

    string filePath;

    private void Start()
    {
        filePath = Application.persistentDataPath + "/saveData.txt";

        health = maxHealth;
        turnManager.UpdateUI();

        string line2 = ReadLine(filePath, 2);
        if(line2 != null)
        {
            gemCount = int.Parse(line2);
        }

        string line4 = ReadLine(filePath, 4);
        //Debug.Log(line4);
        if (line4 == null)
        {
            currentPosition = new Vector2Int(0, 0);
            targetPosition = transform.position;
        }
        if (line4 != null)
        {
            string removeParentheses = line4.Replace("(","").Replace(")","").Trim();
            string[] parts = removeParentheses.Split(',');
            int x = int.Parse(parts[0].Trim());
            int y = int.Parse(parts[1].Trim());
            currentPosition = new Vector2Int(x, y);
            targetPosition = new Vector3(currentPosition.x, 1f, currentPosition.y);
            transform.position = targetPosition;
        }
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        MovePlayer();
        if (!isMoving && enabled)
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
            else if (Input.GetKeyDown(KeyCode.N))
            {
                SaveBoard(currentPosition);
            }
        }
    }
    private void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            animator.SetBool("IsMoving", false);
            if (isMoving == true)
            {
                isMoving = false;
                /*if (watchtower != null && watchtower.gameObject.activeSelf)
                {
                    watchtower.SwapPosition();
                }*/
                turnManager.EndPlayerTurn();
                
            }
        }
    }
    private void AttemptMove(Vector2Int direction)
    {
        Vector2Int targetPosition = currentPosition + direction;

        if (boardManager.IsTileTraversable(targetPosition))
        {
            MoveTo(targetPosition, direction);
        }
    }

    private void MoveTo(Vector2Int newPosition, Vector2Int direction)
    {
        currentPosition = newPosition;
        targetPosition = new Vector3(currentPosition.x, 1f, currentPosition.y);
        PlayAudio(footstepSound);
        animator.SetBool("IsMoving", true);
        isMoving = true;
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }
    public void AddGems(int amount)
    {
        gemCount += amount;
        Debug.Log($"Gems Added: {amount} Total Gems collected: {gemCount}");
        PlayAudio(gemCollect, miningSound.length + .25f);
    }

    public void TakeDamage(int damage)
    {
        animator.SetBool("damaged", true);
        health -= damage;
        PlayAudio(takeDamage);
        healthBar.SetHealth(health);
        StartCoroutine(damgeEnder());
    }
    private void AttemptMining(Vector2Int position)
    {
        if (boardManager.IsMiningNode(position))
        {
            animator.SetBool("IsMining", true);
            PlayAudio(miningSound);
            int successRoll = UnityEngine.Random.Range(0, 100);
            Debug.Log(successRoll);
            if (successRoll < miningSuccessChance)
            {
                int gemsAvailable = boardManager.gemCounts[position.y, position.x];
                if (gemsAvailable > 0)
                {
                    int gemsMined = UnityEngine.Random.Range(1, gemsAvailable + 1);
                    AddGems(gemsMined);

                    gemsAvailable -= gemsMined;
                    boardManager.gemCounts[position.y, position.x] = gemsAvailable;

                    if (gemsAvailable <= 0)
                    {
                        boardManager.gridLayout[position.y, position.x] = 1;
                        boardManager.ReplaceTile(position);
                        Debug.Log($"Space {position.y},{position.x} Depleated");
                    }
                }
            }
            else
            {
                Debug.Log("Failed to mine gems");
            }
            StartCoroutine(miningEnder());
        }
        else
        {
            Debug.Log("Mine at a node");
        }
    }

    private void SaveBoard(Vector2Int position)
    {
        Player player = GetComponent<Player>();
        player.enabled = false;
        if (boardManager.IsSaveNode(position))
        {
            for(int y = 0; y < boardManager.gridLayout.GetLength(0); y++)
            {
                for (int x = 0; x < boardManager.gridLayout.GetLength(1); x++)
                {
                    boardState += boardManager.gridLayout[y, x].ToString();
                    if (x < boardManager.gridLayout.GetLength(1) - 1)
                    {
                        boardState += " ";
                    }
                }
                if (y < boardManager.gridLayout.GetLength(0) - 1)
                {
                    boardState += "\n";
                }
            }
        }

        PlayerPrefs.SetString("boardState", boardState);
        PlayerPrefs.Save();

        string emptyString = "";
        File.WriteAllText(filePath, emptyString);
        File.AppendAllText(filePath, SceneManager.GetActiveScene().name + "\n");
        File.AppendAllText(filePath, gemCount.ToString() + "\n");
        File.AppendAllText(filePath, turnManager.quota.ToString() + "\n");
        File.AppendAllText(filePath, currentPosition.ToString() + "\n");
        File.AppendAllText(filePath, boardState + "\n");

        turnManager.EndGame(false, true);

        /*for (int i = 0; i < boardState.Length; i++)
        {
            Debug.Log(boardState[i]);
        }*/
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
    private IEnumerator miningEnder()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        animator.SetBool("IsMining", false);
        yield return new WaitForSeconds(.5f);
        turnManager.EndPlayerTurn();
    }
    private IEnumerator damgeEnder()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        animator.SetBool("damaged", false);
    }

    string ReadLine(string filePath, int index)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            int currentLine = 1;

            while ((line = reader.ReadLine()) != null)
            {
                if (currentLine == index)
                {
                    return line;
                }
                currentLine++;
            }
        }

        return null;
    }
}
