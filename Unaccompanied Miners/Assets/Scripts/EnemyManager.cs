using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public TurnManager turnManager;
    public GameObject enemyPrefab;
    public GameObject wolfPrefab;
    private List<Vector2Int> spawnPositions;
    private List<Vector2Int[]> patrolPaths;
    private List<Vector2Int> wolfSpawnPositions;
    public string level;
    public string file;

    string folderPath;
    string filePathPaths = "";

    [SerializeField] private int audioPlayersModulo = 3;


    private void Start()
    {
        folderPath = Path.Combine(Application.dataPath, "Paths");
        

        string sceneName = SceneManager.GetActiveScene().name;  
        if (sceneName == "Level 1")
        {
            filePathPaths = Path.Combine(folderPath, "Level1.txt");
        }
        else if (sceneName == "Level 2")
        {
            filePathPaths = Path.Combine(folderPath, "Level2.txt");
        }
        else
        {
            Debug.Log("What");
            //This does not account for the Tutorial levels so I had to make a few changes -Mahliq
        }
        spawnPositions = new List<Vector2Int>();  
        patrolPaths = new List<Vector2Int[]>();
        wolfSpawnPositions = new List<Vector2Int>();

        if (filePathPaths.Length != 0)
        {
            LoadPath(filePathPaths);
        }
        SpawnWolves();
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        int audioIndex = 0;
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, new Vector3(spawnPositions[i].x, 1f, spawnPositions[i].y), Quaternion.Euler(0, -45, 0));
            Enemy enemy = enemyInstance.GetComponent<Enemy>();
            enemy.Initialize(spawnPositions[i], patrolPaths[i]);

            if((i % audioPlayersModulo == 0))
            {
                audioIndex++;
            }
            else
            {
                if (enemy.TryGetComponent(out AudioSource temp))
                {
                    temp.enabled = false;

                }
            }

            turnManager.AddEnemy(enemy);
        }
    }
    private void SpawnWolves()
    {
        foreach (var position in wolfSpawnPositions)
        {
            GameObject wolfInstance = Instantiate(wolfPrefab, new Vector3(position.x, 1f, position.y), Quaternion.Euler(0, 0, 0));
            Wolf wolf = wolfInstance.GetComponent<Wolf>();
            wolf.Initialize(position);
            turnManager.AddWolf(wolf);
        }
    }
    private void LoadPath(string filePath)
    {
        if (!File.Exists(filePath)) 
        {
            Debug.Log("File broken please fix");
        }
        string[] lines = File.ReadAllLines(filePath);
        List<Vector2Int> currentPath = new List<Vector2Int>();
        Vector2Int currentSpawnPosition = new Vector2Int();

        foreach (string line in lines)
        {
            if (line.StartsWith("Enemy"))
            {
                if (currentPath.Count > 0)
                {
                    spawnPositions.Add(currentSpawnPosition);
                    patrolPaths.Add(currentPath.ToArray());
                    currentPath.Clear();
                }
            }
            else if (line.StartsWith("S:"))
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string[] coordinates = parts[1].Split(',');
                    if (coordinates.Length == 2)
                    {
                        int x = int.Parse(coordinates[0]);
                        int y = int.Parse(coordinates[1]);
                        currentSpawnPosition = new Vector2Int(x, y);
                    }
                }
            }
            else if (line.StartsWith("Wolf"))  
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string[] coordinates = parts[1].Trim().Split(','); 
                    if (coordinates.Length == 2)
                    {
                        int x = int.Parse(coordinates[0]);
                        int y = int.Parse(coordinates[1]);
                        wolfSpawnPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(line)) 
            {
                string[] coordinates = line.Split(',');
                if (coordinates.Length == 2)
                {
                    int x = int.Parse(coordinates[0]);
                    int y = int.Parse(coordinates[1]);
                    currentPath.Add(new Vector2Int(x, y));
                }
            }
        }
        if (currentPath.Count > 0)
        {
            spawnPositions.Add(currentSpawnPosition);
            patrolPaths.Add(currentPath.ToArray());
        }
    }
}

