using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public Player player;
    public UserInterface ui;
    BoardManager board;
    public List<Enemy> enemies = new List<Enemy>();
    public List<Wolf> wolves = new List<Wolf>();
    private string currentTurn = "Player's Turn";
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI gemText;
    public int quota = 1;
    public int gems = -1;
    public AudioClip deathMusic;
    private bool happenOnce;

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

    private void Update()
    {
        //quota = ui.quota;
        gems = player.gemCount;
        CheckWin();
    }

    public void StartPlayerTurn()
    {
        currentTurn = "Player's Turn";
        player.enabled = true;
        UpdateUI();
        Debug.Log(player.enabled);
    }

    public void EndPlayerTurn()
    {
        player.enabled = false;
        currentTurn = "Enemy's Turn";
        CheckCollisions();
        UpdateUI();
        StartCoroutine(EnemyTurnCoroutine());
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy); 
    }
    public void AddWolf(Wolf wolf)
    {
        wolves.Add(wolf);
    }
    private IEnumerator EnemyTurnCoroutine()
    {
        bool allEnemiesDone = false;
        yield return new WaitForSeconds(.5f);
        foreach (var enemy in enemies)
        {
            enemy.finished = false;
        }
        foreach (var wolf in wolves)
        {
            wolf.finished = false;
        }
        foreach (var enemy in enemies)
        {
            enemy.PerformTurn();
        }
        foreach (var wolf in wolves)
        {
            wolf.PerformTurn();
        }
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
        foreach (var enemy in enemies)
        {
            CheckCollisions(enemy.currentPosition);
        }

        foreach (var wolf in wolves)
        {
            CheckWolfCollisions(wolf.currentPosition);
        }
        StartPlayerTurn();
    }
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
    private void CheckWolfCollisions(Vector2Int wolfPosition)
    {
        if (wolfPosition == player.currentPosition)
        {
            player.TakeDamage(0);
            var wolf = enemies.FirstOrDefault(wolves => wolves.currentPosition == wolfPosition);
            //wolf.PerformAttack();
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
    private void CheckCollisions(Vector2Int enemyPosition)
    {
        if (enemyPosition == player.currentPosition)
        {
            player.TakeDamage(1);
            var enemy = enemies.FirstOrDefault(enemy => enemy.currentPosition == enemyPosition);
            enemy.PerformAttack();
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
    public void UpdateUI()
    {
        turnText.text = currentTurn;
    }
    
    void CheckWin()
    {
        if (gems >= quota)
        {

            EndGame(true, false);
        }
    }
    
    public void EndGame(bool didYouWin, bool didYouEscape)
    {
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
        else if (!didYouWin) 
        {
            AudioSource music = this.GetComponent<AudioSource>();
            music.clip = deathMusic;
            music.Play();
            Debug.Log("BOOOO");
        }
    }
}
