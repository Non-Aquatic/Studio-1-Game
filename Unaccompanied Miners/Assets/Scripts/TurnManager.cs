using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public Player player;
    public UserInterface ui;
    public List<Enemy> enemies = new List<Enemy>();
    private string currentTurn = "Player's Turn";
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI gemText;
    public int quota = 1;
    public int gems = -1;

    private void Start()
    {
        quota = ui.quota;
        gems = player.gemCount;

        StartPlayerTurn();
        UpdateUI();
    }

    private void Update()
    {
        quota = ui.quota;
        gems = player.gemCount;
        CheckWin();
    }

    public void StartPlayerTurn()
    {
        currentTurn = "Player's Turn";
        player.enabled = true;
        UpdateUI();
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
    private IEnumerator EnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(.5f);

        foreach (var enemy in enemies)
        {
            enemy.PerformTurn();
            CheckCollisions();
            yield return new WaitForSeconds(0.1f); 
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
                if (player.health <= 0)
                {
                    Debug.Log("Player is dead.");
                    EndGame(false); 
                }
                else
                {
                    UpdateUI(); 
                }
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
            EndGame(true);
        }
    }
    
    public void EndGame(bool didYouWin)
    {
        if (didYouWin) 
        {
            ui.winGame();
            PlayerPrefs.SetInt("GemsCount", gems);
            PlayerPrefs.SetInt("Quota", quota);
            PlayerPrefs.Save();
        }
        else if (!didYouWin) 
        {
            Debug.Log("BOOOO");
        }
    }
}
