using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public Player player; //Reference to PLayer
    public UserInterface ui; //Reference to UI interface
    BoardManager board; //Reference to boardManager
    public List<Enemy> enemies = new List<Enemy>(); //List of all Goblins
    public List<Wolf> wolves = new List<Wolf>(); //List of all Wolves
    private string currentTurn = "Player's Turn"; //String for current turn display
    //References to text for turns, health, and current gems
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI gemText;
    //Int for current level's gem quota
    public int quota = 1;
    //Int for gems player currently has
    public int gems = -1;
    //Audio Clip for deathMusic
    public AudioClip deathMusic;
    //Bool in order to prevent the amount of gems from increasing more than it is supposed to 
    private bool happenOnce;

    //Gathers reference to board, starts player with 0 gems, sets up the quota for the current level, and starts as player turn
    private void Start()
    {
        if (TryGetComponent<BoardManager>(out board)) {
            quota = board.quota;
        }
        else
        {
            Debug.LogError($"Cannot find BoardManager in {this.name}");
        }
        happenOnce = false;
        gems = player.gemCount;

        StartPlayerTurn();
        UpdateUI();
    }
    //Constantly whether the player has surpassed the quota
    private void Update()
    {
        //quota = ui.quota;
        gems = player.gemCount;
        CheckWin();
    }
    // Makes it the players turn and reenables the player
    public void StartPlayerTurn()
    {
        currentTurn = "Player's Turn";
        player.enabled = true;
        UpdateUI();
        Debug.Log(player.enabled);
    }
    //Makes it the players turn and disables the player
    public void EndPlayerTurn()
    {
        player.enabled = false;
        currentTurn = "Enemy's Turn";
        CheckCollisions();
        UpdateUI();
        StartCoroutine(EnemyTurnCoroutine());
    }
    //Adds a Goblin to the turn manager
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy); 
    }
    //Adds a Wolf to the turn manager
    public void AddWolf(Wolf wolf)
    {
        wolves.Add(wolf);
    }
    //Method to determine what happens during the enemies turn
    private IEnumerator EnemyTurnCoroutine()
    {
        bool allEnemiesDone = false;
        yield return new WaitForSeconds(.5f);
        //Sets every enemy so they are not done performing thier turn
        foreach (var enemy in enemies)
        {
            enemy.finished = false;
        }
        foreach (var wolf in wolves)
        {
            wolf.finished = false;
        }
        //Allows the Goblins to perform thier turn
        foreach (var enemy in enemies)
        {
            enemy.PerformTurn();
        }
        //Allows the Wolves to perform thier turn
        foreach (var wolf in wolves)
        {
            wolf.PerformTurn();
        }
        //Constantly checks whether all the enemies are done
        while (!allEnemiesDone)
        {
            allEnemiesDone = true;
            foreach (var enemy in enemies)
            {
                if (!enemy.finished)
                {
                    allEnemiesDone = false; 
                    break;
                }
            }
            foreach (var wolf in wolves)
            {
                if (!wolf.finished)
                {
                    allEnemiesDone = false;
                    break;
                }
            }
            yield return null; 
        }
        //Checks for enemy collision a Goblin has with the player
        foreach (var enemy in enemies)
        {
            CheckCollisions(enemy.currentPosition);
        }
        //Checks for enemy collision a Wolf has with the player
        foreach (var wolf in wolves)
        {
            CheckWolfCollisions(wolf.currentPosition);
        }
        //Once everyone is done the Player's turn is activated
        yield return new WaitForSeconds(.5f);
        StartPlayerTurn();
    }
    //Checks if a player landed on the same spot as a Goblin 
    private void CheckCollisions()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.currentPosition == player.currentPosition) 
            {
                player.TakeDamage(1);
                enemy.PerformAttack();
                
                UpdateUI(); 
       
            }
        }
    }
    //Checks if a goblin is in the same space as the player, if so the player takes damage
    private void CheckWolfCollisions(Vector2Int wolfPosition)
    {
        if (wolfPosition == player.currentPosition)
        {
            //No actual damage as it is done in the wolf script
            player.TakeDamage(0);
            var wolf = enemies.FirstOrDefault(wolves => wolves.currentPosition == wolfPosition);
            //wolf.PerformAttack();
            //Ends the game if the player's health drops to 0
            if (player.health <= 0)
            {
                Debug.Log("Player is dead.");
                EndGame(false, false);
            }
            else
            {
                UpdateUI();
            }
        }
    }
    //Checks if a goblin is in the same space as the player, if so the player takes damage
    private void CheckCollisions(Vector2Int enemyPosition)
    {
        if (enemyPosition == player.currentPosition)
        {
            player.TakeDamage(1);
            var enemy = enemies.FirstOrDefault(enemy => enemy.currentPosition == enemyPosition);
            enemy.PerformAttack();
            //Ends the game if the player's health drops to 0
            if (player.health <= 0)
            {
                Debug.Log("Player is dead.");
                EndGame(false, false);
            }
            else
            {
                UpdateUI();
            }
        }
    }
    //Changes the turn text between players and enemies turn
    public void UpdateUI()
    {
        turnText.text = currentTurn;
    }
    //Checks whether you have reached the quota, ends the game if you have
    void CheckWin()
    {
        if (gems >= quota)
        {

            EndGame(true, false);
        }
    }
    //Checks whether you ended the game
    public void EndGame(bool didYouWin, bool didYouEscape)
    {
        //Checks if you won the level, adds current gems to total gems if you have
        if (didYouWin) 
        {
            ui.winGame();
            PlayerPrefs.SetInt("GemsCount", gems);
            PlayerPrefs.SetInt("Quota", quota);
            if (!happenOnce)
            {
                TotalGems.ChangeTotalGems(gems);
                happenOnce = true;
            }
            PlayerPrefs.Save();
        }
        //Checks if you escape the level, adds current gems to total gems if you have
        if (didYouEscape)
        {
            ui.escapeGame();
            PlayerPrefs.SetInt("GemsCount", gems);
            PlayerPrefs.SetInt("Quota", quota);
            if (!happenOnce)
            {
                TotalGems.ChangeTotalGems(gems);
                happenOnce = true;
            }
            PlayerPrefs.Save();
        }
        //If you lost the game then death music plays and gems are not added
        else if (!didYouWin) 
        {
            AudioSource music = this.GetComponent<AudioSource>();
            music.clip = deathMusic;
            music.Play();
            Debug.Log("BOOOO");
        }
    }
}
