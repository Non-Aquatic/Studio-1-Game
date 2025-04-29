using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

//Manages the board in our game
public class BoardManager : MonoBehaviour
{
    //Container for the tiles 
    public GameObject tileContainer; 
    //Different types of tiles prefabs
    public GameObject traversableTilePrefab;
    public GameObject nonTraversableTilePrefab;
    public GameObject miningTilePrefab;
    public GameObject saveTilePrefab;

    public TurnManager turnManager; //Reference to turn manager
    public String sceneName; //Current scene name
    public int quota = 0;  //Number of gems needed to complete level
    int gemSpaces = 0;  //Number of spaces that contain gems
    int gemSpacesLeft = 0; //Number of remaining tiles that should contains gems  but don't
    int gemsLeft = 0; //Number of gems left to assign to tiles
    public int gemsSaved = 0; //Number of gems saved
    //string savedLevel = ""; //Name of saved level

    //Paths to the save file
    string folderPath;
    string filePathPlayer; 
    string filePathBoard;

    //2D array for the grid layout of the game
    //public int[,] gridLayout = {
    //    { 1, 2, 1, 1, 1, 0, 1 },
    //    { 1, 3, 0, 1, 0, 0, 1 },
    //    { 1, 0, 0, 1, 0, 0, 1 },
    //    { 1, 1, 1, 2, 1, 1, 1 },
    //    { 1, 0, 0, 1, 0, 0, 1 },
    //    { 1, 0, 0, 1, 0, 0, 1 },
    //    { 1, 1, 1, 1, 1, 1, 1 },
    //    { 1, 0, 0, 0, 0, 0, 1 },
    //    { 1, 1, 1, 1, 0, 0, 1 },
    //    { 1, 0, 0, 1, 1, 1, 1 },
    //    { 1, 1, 1, 1, 3, 0, 1 },
    //    { 1, 0, 1, 0, 0, 0, 1 },
    //    { 1, 0, 1, 0, 1, 1, 1 },
    //    { 1, 1, 1, 0, 1, 0, 1 },
    //    { 1, 0, 1, 0, 1, 0, 1 },
    //    { 1, 0, 1, 0, 1, 0, 1 },
    //    { 1, 0, 2, 0, 1, 0, 1 },
    //    { 1, 0, 1, 1, 1, 1, 1 },
    //    { 1, 0, 1, 0, 0, 0, 1 },
    //    { 1, 0, 1, 0, 0, 0, 1 },
    //    { 1, 0, 1, 1, 2, 1, 1 },
    //    { 1, 0, 1, 0, 0, 0, 1 },
    //    { 1, 1, 1, 0, 0, 0, 1 },
    //    { 1, 0, 0, 3, 0, 0, 1 },
    //    { 1, 1, 1, 2, 1, 1, 1 }
    //};

    //2D array for the grid
    public int[,] gridLayout;


    //2D array for number of gems on each tile
    public int[,] gemCounts;

    private void Start()
    {
        //Gets the save file and gets data from it at the start
        folderPath = Path.Combine(Application.persistentDataPath, "GameData");
        filePathPlayer = Path.Combine(folderPath, "PlayerData.txt");
        filePathBoard = Path.Combine(folderPath, "LevelData.txt");

        #region Old Loader Code
        /*//sceneName = SceneManager.GetActiveScene().name;
        //FileInfo fileInfoPlayer = new FileInfo(filePathPlayer);
        //FileInfo fileInfoBoard = new FileInfo(filePathBoard);
        ////Reads the saved level data from file if avaliable
        //string line1 = ReadLine(filePathPlayer, 1);
        //if (line1 != null)
        //{
        //    savedLevel = line1;
        //}
        ////If the file is empty
        //if (fileInfoBoard.Length == 0)
        //{
        //    //Default grid layout for level one
        //    if (sceneName == "Level 1")
        //    {
        //        gridLayout = new int[,]{
        //        { 1, 2, 1, 1, 1, 0, 1 },
        //        { 1, 3, 0, 1, 0, 0, 1 },
        //        { 1, 0, 0, 1, 0, 0, 1 },
        //        { 1, 1, 1, 2, 1, 1, 1 },
        //        { 1, 0, 0, 1, 0, 0, 1 },
        //        { 1, 0, 0, 1, 0, 0, 1 },
        //        { 1, 1, 1, 1, 1, 1, 1 },
        //        { 1, 0, 0, 0, 0, 0, 1 },
        //        { 1, 1, 1, 1, 0, 0, 1 },
        //        { 1, 0, 0, 1, 1, 1, 1 },
        //        { 1, 1, 1, 1, 3, 0, 1 },
        //        { 1, 0, 1, 0, 0, 0, 1 },
        //        { 1, 0, 1, 0, 1, 1, 1 },
        //        { 1, 1, 1, 0, 1, 0, 1 },
        //        { 1, 0, 1, 0, 1, 0, 1 },
        //        { 1, 0, 1, 0, 1, 0, 1 },
        //        { 1, 0, 2, 0, 1, 0, 1 },
        //        { 1, 0, 1, 1, 1, 1, 1 },
        //        { 1, 0, 1, 0, 0, 0, 1 },
        //        { 1, 0, 1, 0, 0, 0, 1 },
        //        { 1, 0, 1, 1, 2, 1, 1 },
        //        { 1, 0, 1, 0, 0, 0, 1 },
        //        { 1, 1, 1, 0, 0, 0, 1 },
        //        { 1, 0, 0, 3, 0, 0, 1 },
        //        { 1, 1, 1, 2, 1, 1, 1 }
        //        };

        //    }
        //    //Default grid layout for level two
        //    else if (sceneName == "Level 2")
        //    {
        //        gridLayout = new int[,]{
        //        { 1, 1, 1, 1, 2, 1, 3 },
        //        { 0, 0, 0, 0, 0, 0, 1 },
        //        { 0, 0, 0, 0, 0, 0, 1 },
        //        { 1, 2, 1, 1, 1, 1, 1 },
        //        { 1, 0, 0, 1, 0, 0, 1 },
        //        { 1, 0, 0, 2, 0, 0, 1 },
        //        { 1, 1, 1, 1, 1, 2, 1 },
        //        { 0, 0, 0, 0, 0, 0, 1 },
        //        { 1, 1, 1, 1, 0, 0, 1 },
        //        { 1, 0, 0, 1, 0, 0, 1 },
        //        { 1, 1, 1, 1, 2, 0, 1 },
        //        { 1, 0, 1, 0, 1, 0, 2 },
        //        { 1, 0, 1, 0, 1, 0, 1 },
        //        { 1, 1, 1, 1, 1, 1, 1 },
        //        { 1, 0, 1, 0, 1, 0, 1 },
        //        { 1, 0, 1, 2, 1, 1, 1 },
        //        { 1, 0, 2, 0, 1, 0, 2 },
        //        { 2, 0, 1, 1, 1, 0, 1 },
        //        { 1, 1, 1, 1, 1, 1, 1 },
        //        { 0, 0, 1, 0, 0, 0, 1 },
        //        { 0, 0, 1, 1, 2, 1, 1 },
        //        { 0, 0, 1, 0, 1, 0, 1 },
        //        { 1, 1, 1, 0, 1, 0, 1 },
        //        { 1, 0, 0, 0, 1, 0, 1 },
        //        { 2, 1, 1, 1, 1, 1, 1 }
        //        };
        //    }
            
        //}
        //else
        //{
        //    //If a save file exists, read the grid layout from the file
        //    using (StreamReader reader = new StreamReader(filePathBoard))
        //    {
        //        int startLine = 2;
        //        string line;
        //        int currentLine = 1;
        //        int row = 0;

        //        while((line = reader.ReadLine()) != null)
        //        {
        //            string[] numbers = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //            if (currentLine >= startLine)
        //            {
        //                for (int col = 0; col < numbers.Length && col < gridLayout.GetLength(1); col++)
        //                {
        //                    if (int.TryParse(numbers[col], out int value))
        //                    {
        //                        gridLayout[row, col] = value;
        //                    }
        //                }

        //                row++;
        //                if (row >= gridLayout.GetLength(0))
        //                    break;
        //            }

        //            currentLine++;
        //        }
        //    }

        //}
        ////Gems acquired loaded from save
        //string line2 = ReadLine(filePathPlayer, 2);
        //if (line2 != null)
        //{
        //    gemsSaved = int.Parse(line2);
        //}
        ////Level quota loaded from save
        //string line3 = ReadLine(filePathPlayer, 3);
        //if (line3 != null)
        //{
        //    quota = int.Parse(line3);
        //}
        ////If not saved, use default from level
        //else if (SceneManager.GetActiveScene().name == "Level 1")
        //{
        //    quota = 25;
        //}
        //else
        //{
        //    quota = 35;
        //}

        //Generates the board and gem counts
        //GenerateBoard();
        //GenerateGemCounts();*/
        #endregion
    }

    //Sets the grid layout to the grid when loading in
    public void SetGrid(int[,] newGrid)
    {
        gridLayout = newGrid;
    }
    
    //Generates the board based on gridLayout
    public void GenerateBoard()
    {
        int rows = gridLayout.GetLength(0);
        int columns = gridLayout.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject tilePrefab;
                // Choose the correct tile prefab for each space based on gridLayout values
                //1 is normal walkable tiles
                if (gridLayout[y, x] == 1)
                {
                    tilePrefab = traversableTilePrefab;
                }
                //2 is minable tiles
                else if (gridLayout[y, x] == 2)
                {
                    gemSpaces++;
                    tilePrefab = miningTilePrefab; 
                }
                //3 is saving tiles
                else if (gridLayout[y, x] == 3)
                {
                    tilePrefab = saveTilePrefab;
                }
                //All of others or 0 are unwalkable tiles
                else
                {
                    tilePrefab = nonTraversableTilePrefab;
                }
                //Instantiates the tiles and sets it as a child of tileCounter
                GameObject tiles = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                tiles.transform.SetParent(tileContainer.transform);
                tiles.name = "Tile"+x+","+y;
            }
        }
    }
    //Generates the gem nodes with a random amount of gems per node
    public void GenerateGemCounts()
    {
        //If no gems are saved, make the amount of gems possible quota + 5
        if (gemsSaved == 0)
        {
            gemsLeft = quota + 5;
        }
        //If we have some gems saved, make the amount of gems possible quota + 5 - however many we have
        if (gemsSaved > 0)
        {
            gemsLeft = (quota +5)- gemsSaved;
        }

        gemSpacesLeft = gemSpaces;
        // Assign gems to mining tiles randomly depending on the amount of gem tiles there are 
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
    }
    //Replaces the tiles at specific locations
    //For example: if a Gem node runs out of gems it becomes a regular one
    public void ReplaceTile(Vector2Int position)
    {
        GameObject replacedTile = GameObject.Find("Tile"+position.x+","+position.y);

        Destroy(replacedTile);

        GameObject newTileInstance = Instantiate(traversableTilePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        newTileInstance.transform.SetParent(tileContainer.transform);
        newTileInstance.name = "Tile" + position.x + "," + position.y;
    }
    //Checks if you can walk on top of the tiles
    public bool IsTileTraversable(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridLayout.GetLength(1) || position.y < 0 || position.y >= gridLayout.GetLength(0))
            return false;
        return gridLayout[position.y, position.x] == 1 || gridLayout[position.y, position.x] == 2 || gridLayout[position.y, position.x] == 3;
    }
    //Checks if you cna mine at that tile
    public bool IsMiningNode(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridLayout.GetLength(1) || position.y < 0 || position.y >= gridLayout.GetLength(0))
            return false;

        return gridLayout[position.y, position.x] == 2; 
    }
    //Checks if you can save at that tile
    public bool IsSaveNode(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridLayout.GetLength(1) || position.y < 0 || position.y >= gridLayout.GetLength(0))
            return false;

        return gridLayout[position.y, position.x] == 3;
    }
    //Reads a designated line from the save file
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
