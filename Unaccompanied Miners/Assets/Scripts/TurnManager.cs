using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public Player player;
    public List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        player.enabled = true;
    }

    public void EndPlayerTurn()
    {
        player.enabled = false;
        StartCoroutine(EnemyTurnCoroutine());
    }
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy); 
    }
    private IEnumerator EnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(1f);

        foreach (var enemy in enemies)
        {
            enemy.PerformTurn(); 
            yield return new WaitForSeconds(0.5f); 
        }

        StartPlayerTurn();
    }
}
