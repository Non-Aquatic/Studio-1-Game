using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public Player player;
    public List<Enemy> enemies = new List<Enemy>();
    private string currentTurn = "Player's Turn";
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI gemText;

    private void Start()
    {
        StartPlayerTurn();
        UpdateUI();
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
        yield return new WaitForSeconds(.75f);

        foreach (var enemy in enemies)
        {
            enemy.PerformTurn();
            CheckCollisions();
            yield return new WaitForSeconds(0.25f); 
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
        healthText.text = "Health: "+player.health;
        gemText.text = "Gems: " + player.gemCount;
    }
    //IDK what to do here for right now
    public void EndGame(bool didYouWin)
    {
        if (didYouWin) 
        {
            Debug.Log("Yay");
        }
        else if (!didYouWin) 
        {
            Debug.Log("BOOOO");
        }


        player.enabled = false;
    }
}
