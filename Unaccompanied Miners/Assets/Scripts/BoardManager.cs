using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    public GameObject tileContainer; 
    public GameObject traversableTilePrefab;
    public GameObject nonTraversableTilePrefab;
    public GameObject miningTilePrefab;
    public GameObject saveTilePrefab;
    public TurnManager turnManager;
    private String sceneName;
    public int quota = 0;
    int gemSpaces = 0;
    int gemSpacesLeft = 0;
    int gemsLeft = 0;
    int gemsSaved = 0;

    string filePath;

    public int[,] gridLayout = {
        { 1, 2, 1, 1, 1, 0, 1 },
        { 1, 3, 0, 1, 0, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 1 },
        { 1, 1, 1, 2, 1, 1, 1 },
        { 1, 0, 0, 1, 0, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 0, 0, 1 },
        { 1, 0, 0, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 3, 0, 1 },
        { 1, 0, 1, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 1, 1, 1 },
        { 1, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1 },
        { 1, 0, 2, 0, 1, 0, 1 },
        { 1, 0, 1, 1, 1, 1, 1 },
        { 1, 0, 1, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 0, 0, 1 },
        { 1, 0, 1, 1, 2, 1, 1 },
        { 1, 0, 1, 0, 0, 0, 1 },
        { 1, 1, 1, 0, 0, 0, 1 },
        { 1, 0, 0, 3, 0, 0, 1 },
        { 1, 1, 1, 2, 1, 1, 1 }
    };

    public int[,] gemCounts;

    private void Start()
    {
        filePath = Application.persistentDataPath + "/saveData.txt";
        
        sceneName = SceneManager.GetActiveScene().name;

        FileInfo fileInfo = new FileInfo(filePath);
        
        if (fileInfo.Length == 0)
        {
            if (sceneName == "Level 1")
            {
                gridLayout = new int[,]{
                { 1, 2, 1, 1, 1, 0, 1 },
                { 1, 3, 0, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 1, 1, 2, 1, 1, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 3, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 1, 1 },
                { 1, 1, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 2, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 1, 2, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 0, 0, 0, 1 },
                { 1, 0, 0, 3, 0, 0, 1 },
                { 1, 1, 1, 2, 1, 1, 1 }
                };

            }
            else
            {
                gridLayout = new int[,]{
                { 1, 1, 1, 1, 2, 1, 3 },
                { 0, 0, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 2, 1, 1, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 3, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 1, 1 },
                { 1, 1, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 2, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 1, 2, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 0, 0, 0, 1 },
                { 1, 0, 0, 3, 0, 0, 1 },
                { 1, 1, 1, 2, 1, 1, 1 }
                };
            }
            
        }
        else
        {
            gridLayout = new int[,]{
                { 1, 1, 1, 1, 2, 1, 3 },
                { 0, 0, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 2, 1, 1, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 3, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 1, 1 },
                { 1, 1, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 2, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 1, 1, 2, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 0, 0, 0, 1 },
                { 1, 0, 0, 3, 0, 0, 1 },
                { 1, 1, 1, 2, 1, 1, 1 }
                };
            using (StreamReader reader = new StreamReader(filePath))
            {
                int startLine = 5;
                string line;
                int currentLine = 1;
                int row = 0;

                while((line = reader.ReadLine()) != null)
                {
                    string[] numbers = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (currentLine >= startLine)
                    {
                        for (int col = 0; col < numbers.Length && col < gridLayout.GetLength(1); col++)
                        {
                            if (int.TryParse(numbers[col], out int value))
                            {
                                gridLayout[row, col] = value;
                            }
                        }

                        row++;
                        if (row >= gridLayout.GetLength(0))
                            break;
                    }

                    currentLine++;
                }
            }

        }

        string line2 = ReadLine(filePath, 2);
        if (line2 != null)
        {
            gemsSaved = int.Parse(line2);
        }

        string line3 = ReadLine(filePath, 3);
        if (line3 != null)
        {
            quota = int.Parse(line3);
        }


        GenerateBoard();
        GenerateGemCounts();
    }
    private void GenerateBoard()
    {
        int rows = gridLayout.GetLength(0);
        int columns = gridLayout.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject tilePrefab;
                if (gridLayout[y, x] == 1)
                {
                    tilePrefab = traversableTilePrefab;
                }
                else if (gridLayout[y, x] == 2)
                {
                    gemSpaces++;
                    tilePrefab = miningTilePrefab; 
                }
                else if (gridLayout[y, x] == 3)
                {
                    tilePrefab = saveTilePrefab;
                }
                else
                {
                    tilePrefab = nonTraversableTilePrefab;
                }
                GameObject tiles = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                tiles.transform.SetParent(tileContainer.transform);
                tiles.name = "Tile"+x+","+y;
            }
        }
    }
    private void GenerateGemCounts()
    {
        if (gemsSaved == 0)
        {
            gemsLeft = quota;
        }
        if (gemsSaved > 0)
        {
            gemsLeft = quota - gemsSaved;
        }

        gemSpacesLeft = gemSpaces;

        gemCounts = new int[gridLayout.GetLength(0), gridLayout.GetLength(1)];
        for (int y = 0; y < gemCounts.GetLength(0); y++)
        {
            for (int x = 0; x < gemCounts.GetLength(1); x++)
            {
                if (gridLayout[y, x] == 2)
                {
                    if (gemSpacesLeft > 1 && gemsLeft > 0)
                    {
                        int temp = UnityEngine.Random.Range(gemsLeft/(gemSpaces), (quota/2) % gemsLeft);
                        Debug.Log($"Assigning {temp} Gems to space");
                        gemCounts[y, x] = temp;
                        gemsLeft -= temp;
                        gemSpacesLeft--;

                    }
                    else
                    {
                        Debug.Log($"Assigning {gemsLeft} Gems to space");
                        gemCounts[y, x] = gemsLeft;
                        gemsLeft -= gemsLeft;
                        gemSpacesLeft--;
                    }

                    Debug.Log($"Total Gems: {quota} Total Gem Spaces: {gemSpaces} Gems Left to Assign: {gemsLeft} Gem Spaces Left to Fill: {gemSpacesLeft}");

                }
                else
                {
                    gemCounts[y, x] = 0;
                }
            }
        }

        //string temp = 


    }
    public void ReplaceTile(Vector2Int position)
    {
        GameObject replacedTile = GameObject.Find("Tile"+position.x+","+position.y);

        Destroy(replacedTile);

        GameObject newTileInstance = Instantiate(traversableTilePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        newTileInstance.transform.SetParent(tileContainer.transform);
        newTileInstance.name = "Tile" + position.x + "," + position.y;
    }
    public bool IsTileTraversable(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridLayout.GetLength(1) || position.y < 0 || position.y >= gridLayout.GetLength(0))
            return false;
        return gridLayout[position.y, position.x] == 1 || gridLayout[position.y, position.x] == 2 || gridLayout[position.y, position.x] == 3;
    }
    public bool IsMiningNode(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridLayout.GetLength(1) || position.y < 0 || position.y >= gridLayout.GetLength(0))
            return false;

        return gridLayout[position.y, position.x] == 2; 
    }
    public bool IsSaveNode(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridLayout.GetLength(1) || position.y < 0 || position.y >= gridLayout.GetLength(0))
            return false;

        return gridLayout[position.y, position.x] == 3;
    }

    string ReadLine(string filePath, int index)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            int currentLine = 1;

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
}
