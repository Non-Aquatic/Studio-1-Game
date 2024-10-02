using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public class PatrolPath
    {
        public Vector2Int[] path;
    }
    public TurnManager turnManager;
    public GameObject enemyPrefab; 
    public Vector2Int[] spawnPositions; 
    public PatrolPath[] patrolPaths;

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, new Vector3(spawnPositions[i].x, 0.5f, spawnPositions[i].y), Quaternion.identity);
            Enemy enemy = enemyInstance.GetComponent<Enemy>();
            enemy.Initialize(spawnPositions[i], patrolPaths[i].path);
            turnManager.AddEnemy(enemy);
        }
    }
}
