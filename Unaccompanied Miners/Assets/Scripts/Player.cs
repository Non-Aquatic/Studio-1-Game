using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Transactions;

public class Player : MonoBehaviour
{
    public BoardManager boardManager; //Reference to board manager
    public TurnManager turnManager; //Reference to turn manager
    public Vector2Int currentPosition; //Current position of player
    public float moveSpeed = 5f; //Speed of player
    public Vector3 targetPosition; //Next position of player
    private Animator animator; //Animator for the player
    //public Watchtower watchtower;

    //Player stats
    public int gemCount; //Gems acquired 
    public int health = 1; //Player current health
    public int maxHealth = 10; //Player maximum health
    public HealthBar healthBar; //Reference to health bar
    private int miningSuccessChance = 80; //Chance of successfully getting gems after mining attempt

    //Audio clips for various actions
    public AudioClip miningSound; //Assigned to mining audio clip in inspector, plays on mine
    public AudioClip gemCollect; //Assigned to crystal_pickup audio clip in inspector, plays on successful mine
    public AudioClip takeDamage; //Assigned to hit audio clip in inspector, plays on damage taken
    public AudioClip footstepSound; //Assigned to footsetp audio clip in inspector, plays on movement

    public float audioVolume = .5f; // Audio volume, 0-1f.
    public bool isMoving = false; //Bool for whether player is moving
    public string boardState = ""; //Board state

    string filePath; //Path to the save file
    private SaveLoadScript saveLoadScript;

    private void Start()
    {
        //Sets up path to the save file
        filePath = Application.persistentDataPath + "/saveData.txt";
        saveLoadScript = GetComponent<SaveLoadScript>();

        //Initializes health to maximum
        health = maxHealth;
        turnManager.UpdateUI();

        //Reads saved gem count from file if avaliable
        /*string line2 = ReadLine(filePath, 2);
        if(line2 != null)
        {
            gemCount = int.Parse(line2);
        }

        //Reads saved player position from file if avaliable
        string line4 = ReadLine(filePath, 4);
        //If position is not saved, start at (0, 0)
        if (line4 == null)
        {
            currentPosition = new Vector2Int(0, 0);
            targetPosition = transform.position;
        }
        //If position is saved, places player at that position
        if (line4 != null)
        {
            string removeParentheses = line4.Replace("(","").Replace(")","").Trim();
            string[] parts = removeParentheses.Split(',');
            int x = int.Parse(parts[0].Trim());
            int y = int.Parse(parts[1].Trim());
            currentPosition = new Vector2Int(x, y);
            targetPosition = new Vector3(currentPosition.x, 1f, currentPosition.y);
            transform.position = targetPosition;
        }*/
        // Gets animator component
        animator = GetComponent<Animator>();
        //Saves initial board state
        //SaveBoard(currentPosition);
    }

    private void Update()
    {
        //Handles movment
        MovePlayer();
        //Handles player inputs for movement, mining, and saving
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
                saveLoadScript.SaveBoard(currentPosition);
                Escape();
            }
        }
    }
    //Moves player to new location smoothly
    private void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        //Once it has reached target position
        if (transform.position == targetPosition)
        {
            //Moving animation set to false and player's turn ends
            animator.SetBool("IsMoving", false);
            if (isMoving == true)
            {
                isMoving = false;
                turnManager.EndPlayerTurn();
            }
        }
    }
    //Attempts to move the move in the specified direction 
    private void AttemptMove(Vector2Int direction)
    {
        Vector2Int targetPosition = currentPosition + direction;

        //Checks if specified tile is traversable
        if (boardManager.IsTileTraversable(targetPosition))
        {
            MoveTo(targetPosition, direction);
        }
    }
    //Sets new position for player to move towards
    private void MoveTo(Vector2Int newPosition, Vector2Int direction)
    {
        currentPosition = newPosition;
        targetPosition = new Vector3(currentPosition.x, 1f, currentPosition.y);
        //Plays footstep audio
        PlayAudio(footstepSound);
        //Moving animation set to true with direction
        animator.SetBool("IsMoving", true);
        isMoving = true;
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }
    //Adds a number of gems to player's inventory
    public void AddGems(int amount)
    {
        gemCount += amount;
        Debug.Log($"Gems Added: {amount} Total Gems collected: {gemCount}");
        //Plays gem collection sound
        PlayAudio(gemCollect, miningSound.length + .25f);
    }
    //Handles player taking damage
    public void TakeDamage(int damage)
    {
        //Damage animation set to true
        animator.SetBool("damaged", true);
        //Player loses health
        health -= damage;
        //Plays damage sound
        PlayAudio(takeDamage);
        //Updates health bar UI
        healthBar.SetHealth(health);
        //Ends the game if health is 0
        if (health <= 0)
        {
            Debug.Log("Player is dead.");
            turnManager.EndGame(false, false);
        }
        //Starts coroutine to end damage animation
        StartCoroutine(damgeEnder());
    }
    //Attempts to mine gems at a specific position
    private void AttemptMining(Vector2Int position)
    {
        //Checks if the player is on a mining tile
        if (boardManager.IsMiningNode(position))
        {
            //Mining animation is set to true
            animator.SetBool("IsMining", true);
            //Plays mining sound
            PlayAudio(miningSound);
            //Rolls for mining success
            int successRoll = UnityEngine.Random.Range(0, 100);
            Debug.Log(successRoll);
            //If mining is sucessful
            if (successRoll < miningSuccessChance)
            {
                int gemsAvailable = boardManager.gemCounts[position.y, position.x];
                //Checks how many gems are left at that spot
                if (gemsAvailable > 0)
                {
                    //Gives player a random number of gems
                    int gemsMined = UnityEngine.Random.Range(1, gemsAvailable + 1);
                    AddGems(gemsMined);

                    //Removes them from the tile
                    gemsAvailable -= gemsMined;
                    boardManager.gemCounts[position.y, position.x] = gemsAvailable;

                    //If there are no gems left in that tile, replace it with a normal tile
                    if (gemsAvailable <= 0)
                    {
                        boardManager.gridLayout[position.y, position.x] = 1;
                        boardManager.ReplaceTile(position);
                        Debug.Log($"Space {position.y},{position.x} Depleated");
                    }
                }
            }
            //If mining has failed, nothing happens
            else
            {
                Debug.Log("Failed to mine gems");
            }
            //Starts coroutine to end mining animation
            StartCoroutine(miningEnder());
        }
        else
        {
            Debug.Log("Mine at a node");
        }
    }

    //Saves the current board to a file and playerPrefs
    /*public void SaveBoard(Vector2Int position)
    {
        boardState = string.Empty;
        //Gets the entire board in the form of a string
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

        //Saves to PlayerPrefs
        PlayerPrefs.SetString("boardState", boardState);
        PlayerPrefs.Save();

        //Saves to the save file as well
        string emptyString = "";
        File.WriteAllText(filePath, string.Empty);
        File.AppendAllText(filePath, SceneManager.GetActiveScene().name + "\n");
        File.AppendAllText(filePath, gemCount.ToString() + "\n");
        File.AppendAllText(filePath, turnManager.quota.ToString() + "\n");
        File.AppendAllText(filePath, currentPosition.ToString() + "\n");
        File.AppendAllText(filePath, boardState + "\n");
    }*/

    //Plays the given audio
    private void PlayAudio(AudioClip audioInput)
    {
        if (this.TryGetComponent(out AudioSource temp))
        {
            temp.PlayOneShot(audioInput, audioVolume);
        }

    }
    //Plays the given audio with a delay
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
    //Coroutine to end the mining animation after completion
    private IEnumerator miningEnder()
    {
        //Waits until animation is finished
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        //Mining animation is set to false
        animator.SetBool("IsMining", false);
        yield return new WaitForSeconds(.5f);
        //Ends players turn
        turnManager.EndPlayerTurn();
    }
    //Coroutine to end the damage animation after completion
    private IEnumerator damgeEnder()
    { 
        //Waits until animation is finished
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        //Damage animation is set to false
        animator.SetBool("damaged", false);
    }
    //Reads a specific line from the save file
    string ReadLine(string filePath, int index)
    {
        //Checks if the file exists
        if (!File.Exists(filePath))
        {
            return null;
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            int currentLine = 1;

            //Gets the needed line from the file
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
    //Handles the player escaping the mines
    public void Escape()
    {
        Player player = GetComponent<Player>();
        //Disables all controls
        player.enabled = false;
        //Ends the game with and escape
        turnManager.EndGame(false, true);
    }
}
