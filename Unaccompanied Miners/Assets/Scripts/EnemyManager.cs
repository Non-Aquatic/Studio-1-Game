using UnityEngine;

//Manages the simple goblin enemies
public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public class PatrolPath //The patrol path for each simple enemy
    {
        public Vector2Int[] path;
    }
    public TurnManager turnManager; //Reference to turn manager
    public GameObject enemyPrefab; //Prefab for enemy
    public Vector2Int[] spawnPositions; //Spawn positions for all enemies
    public PatrolPath[] patrolPaths; //Array that holds all patrol paths for all enemies

    private void Start()
    {
        SpawnEnemies();
    }
    //Spawns all enemies with their paths and spawn positions and adds them to the turn manager
    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, new Vector3(spawnPositions[i].x, 1f, spawnPositions[i].y), Quaternion.Euler(0, -45, 0));
            Enemy enemy = enemyInstance.GetComponent<Enemy>();
            enemy.Initialize(spawnPositions[i], patrolPaths[i].path);
            turnManager.AddEnemy(enemy);
        }
    }
}
