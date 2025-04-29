using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    //Turn manager reference
    public TurnManager turnManager;
    //Prefabs for both Wolfs and Goblins
    public GameObject enemyPrefab;
    public GameObject wolfPrefab;
    //Spawn and patrol positions for Goblins
    private List<Vector2Int> spawnPositions;
    private List<Vector2Int[]> patrolPaths;
    //Spawn positions for Wolves
    private List<Vector2Int> wolfSpawnPositions;
    //String for what level and location of files
    public string level;
    public string file;
    string folderPath;
    string filePathPaths = "";

    [SerializeField] private int audioPlayersModulo = 3;


    private void Start()
    {
        //Chooses the correct enemy file depending on the scene name 
        folderPath = Path.Combine(Application.persistentDataPath, "Paths");
        

        string sceneName = SceneManager.GetActiveScene().name;  
        if (sceneName == "Level 1")
        {
            filePathPaths = Path.Combine(Application.streamingAssetsPath, "Level1.txt");
        }
        else if (sceneName == "Level 2")
        {
            filePathPaths = Path.Combine(Application.streamingAssetsPath, "Level2.txt");
        }
        else if (sceneName == "Level 3")
        {
            filePathPaths = Path.Combine(Application.streamingAssetsPath, "Level3.txt");
        }
        else if (sceneName == "Level 4")
        {
            filePathPaths = Path.Combine(Application.streamingAssetsPath, "Level4.txt");
        }
        else if (sceneName == "Tutorial 2")
        {
            filePathPaths = Path.Combine(Application.streamingAssetsPath, "Tutorial2.txt");
        }
        else
        {
            Debug.Log("What");
            //This does not account for the Tutorial levels so I had to make a few changes -Mahliq
        }
        //Creates lists for all enemy necessities 
        spawnPositions = new List<Vector2Int>();  
        patrolPaths = new List<Vector2Int[]>();
        wolfSpawnPositions = new List<Vector2Int>();
        //If there is something in the enemy file, load it in
        if (filePathPaths.Length != 0)
        {
            LoadPath(filePathPaths);
        }
        //Actually Spawns the Goblins and Wolves
        SpawnWolves();
        SpawnEnemies();
    }
    //Spawns the Goblins
    private void SpawnEnemies()
    {
        //Grabs ths spawn positions of the goblins and instantiates them with there patrol paths
        int audioIndex = 0;
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, new Vector3(spawnPositions[i].x, 1f, spawnPositions[i].y), Quaternion.Euler(0, -45, 0));
            Enemy enemy = enemyInstance.GetComponent<Enemy>();
            enemy.Initialize(spawnPositions[i], patrolPaths[i]);
            //Distributes a limited amount of audio sources to enemies
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
    //Spawns the wolves
    private void SpawnWolves()
    {
        foreach (var position in wolfSpawnPositions)
        {
            //Grabs ths spawn positions of the wolves and instantiates them with that
            GameObject wolfInstance = Instantiate(wolfPrefab, new Vector3(position.x, 1f, position.y), Quaternion.Euler(0, -45, 0));
            Wolf wolf = wolfInstance.GetComponent<Wolf>();
            wolf.Initialize(position);
            turnManager.AddWolf(wolf);
        }
    }
    //Loads the enemies through a file
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
            //Finshes the previous enemy and creates a new Goblin
            if (line.StartsWith("Enemy"))
            {
                if (currentPath.Count > 0)
                {
                    spawnPositions.Add(currentSpawnPosition);
                    patrolPaths.Add(currentPath.ToArray());
                    currentPath.Clear();
                }
            }
            //Creates a new spawn position for a goblin
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
            //Creates a new wolf enemy
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
            //If not empty at the coordinates to the goblin enemy as a patrol point
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
        //If path for goblin enemy is not empty, finish the last goblin enemy and add to spawn and patrol paths
        if (currentPath.Count > 0)
        {
            spawnPositions.Add(currentSpawnPosition);
            patrolPaths.Add(currentPath.ToArray());
        }
    }
}

