using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Transactions;
using UnityEngine.UI;
using UnityEditor.TestTools.CodeCoverage;

public class SaveLoadScript : MonoBehaviour
{
    public Player player;
    public string boardState = ""; //Board state
    string folderPath; // Path to the GameData folder
    string filePathPlayer; //Path to the Player save file
    string filePathBoard; //Path to the level save file
    string lvlPath;

    public BoardManager boardManager;
    private String sceneName; //Current scene name
    public int quota = 0;
    int gemsSaved = 0;
    string savedLevel = ""; //Name of saved level

    //2D array for the grid layout of the game
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

    //public int[,] gridLayout;

    // Start is called before the first frame update
    void Start()
    {
        folderPath = Path.Combine(Application.dataPath, "GameData");
        filePathPlayer = Path.Combine(folderPath, "PlayerData.txt");
        filePathBoard = Path.Combine(folderPath, "LevelData.txt");
        sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Main Menu")
        {
            //player = new Player();
            Button button = GetComponent<Button>();
        }
        else
        {
            player = GetComponent<Player>();
            ReadFile();
        }


        /*//Reads saved gem count from file if avaliable
        string line2 = ReadLine(filePathPlayer, 2);
        if (line2 != null)
        {
            player.gemCount = int.Parse(line2);
            gemsSaved = int.Parse(line2);
        }

        //Reads saved player position from file if avaliable
        string line1Level = ReadLine(filePathBoard, 1);

        //If position is not saved, start at (0, 0)
        if (line1Level == null)
        {
            player.currentPosition = new Vector2Int(0, 0);
            player.targetPosition = transform.position;
        }
        //If position is saved, places player at that position
        if (line1Level != null)
        {
            string removeParentheses = line1Level.Replace("(", "").Replace(")", "").Trim();
            string[] parts = removeParentheses.Split(',');
            int x = int.Parse(parts[0].Trim());
            int y = int.Parse(parts[1].Trim());
            player.currentPosition = new Vector2Int(x, y);
            player.targetPosition = new Vector3(player.currentPosition.x, 1f, player.currentPosition.y);
            transform.position = player.targetPosition;
        }*/

        /*sceneName = SceneManager.GetActiveScene().name;
        FileInfo fileInfoPlayer = new FileInfo(filePathPlayer);
        FileInfo fileInfoBoard = new FileInfo(filePathBoard);
        //Reads the saved level data from file if avaliable
        string line1 = ReadLine(filePathPlayer, 1);
        if (line1 != null)
        {
            savedLevel = line1;
        }
        //If the file is empty
        ReadBoard(fileInfoBoard);

        //Level quota loaded from save
        string line3 = ReadLine(filePathPlayer, 3);
        if (line3 != null)
        {
            quota = int.Parse(line3);
        }

        //If not saved, use default from level
        else if (SceneManager.GetActiveScene().name == "Level 1")
        {
            quota = 25;
        }
        else
        {
            quota = 35;
        }

        boardManager.sceneName = sceneName;
        boardManager.quota = quota;
        boardManager.gemsSaved = gemsSaved;

        boardManager.SetGrid(gridLayout);

        boardManager.GenerateBoard();
        boardManager.GenerateGemCounts();

        SaveBoard(player.currentPosition);*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ReadFile()
    {
        //Reads saved gem count from file if avaliable
        string line2 = ReadLine(filePathPlayer, 2);
        if (line2 != null)
        {
            player.gemCount = int.Parse(line2);
            gemsSaved = int.Parse(line2);
        }

        //Reads saved player position from file if avaliable
        string line1Level = ReadLine(filePathBoard, 1);

        //If position is not saved, start at (0, 0)
        if (line1Level == null)
        {
            player.currentPosition = new Vector2Int(0, 0);
            player.targetPosition = transform.position;
        }
        //If position is saved, places player at that position
        if (line1Level != null)
        {
            string removeParentheses = line1Level.Replace("(", "").Replace(")", "").Trim();
            string[] parts = removeParentheses.Split(',');
            int x = int.Parse(parts[0].Trim());
            int y = int.Parse(parts[1].Trim());
            player.currentPosition = new Vector2Int(x, y);
            player.targetPosition = new Vector3(player.currentPosition.x, 1f, player.currentPosition.y);
            transform.position = player.targetPosition;
        }

        sceneName = SceneManager.GetActiveScene().name;
        FileInfo fileInfoPlayer = new FileInfo(filePathPlayer);
        FileInfo fileInfoBoard = new FileInfo(filePathBoard);
        //Reads the saved level data from file if avaliable
        string line1 = ReadLine(filePathPlayer, 1);
        if (line1 != null)
        {
            savedLevel = line1;
        }
        ReadBoard(fileInfoBoard);

        //Level quota loaded from save
        string line3 = ReadLine(filePathPlayer, 3);
        if (line3 != null)
        {
            quota = int.Parse(line3);
        }

        //If not saved, use default from level
        else if (SceneManager.GetActiveScene().name == "Level 1")
        {
            quota = 25;
        }
        else
        {
            quota = 35;
        }

        boardManager.sceneName = sceneName;
        boardManager.quota = quota;
        boardManager.gemsSaved = gemsSaved;

        boardManager.SetGrid(gridLayout);

        boardManager.GenerateBoard();
        boardManager.GenerateGemCounts();

        SaveBoard(player.currentPosition);
    }
    string ReadLine(string filePath, int index)
    {
        //Checks if the file exists
        if (!File.Exists(filePath))
        {
            return null;
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            int currentLine = 1;

            //Gets the needed line from the file
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
    void ReadBoard(FileInfo levelData)
    {
        if (levelData.Length == 0)
        {
            /*//Default grid layout for level one
            if (savedLevel == "Level 1")
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
            //Default grid layout for level two
            else if (savedLevel == "Level 2")
            {
                gridLayout = new int[,]{
                { 1, 1, 1, 1, 2, 1, 3 },
                { 0, 0, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 1 },
                { 1, 2, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 0, 0, 2, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 2, 1 },
                { 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 0, 0, 1 },
                { 1, 1, 1, 1, 2, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 2 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 2, 1, 1, 1 },
                { 1, 0, 2, 0, 1, 0, 2 },
                { 2, 0, 1, 1, 1, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1 },
                { 0, 0, 1, 0, 0, 0, 1 },
                { 0, 0, 1, 1, 2, 1, 1 },
                { 0, 0, 1, 0, 1, 0, 1 },
                { 1, 1, 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 1 },
                { 2, 1, 1, 1, 1, 1, 1 }
                };
            }*/
            //string loadingScene = PlayerPrefs.GetString(currentScene);
            if(sceneName == "Level 1")
            {
                lvlPath = Path.Combine(folderPath, "Level-1.txt");
            }
            if (sceneName == "Level 2")
            {
                lvlPath = Path.Combine(folderPath, "Level-2.txt");
            }

            using (StreamReader reader = new StreamReader(lvlPath))
            {
                string line;
                int row = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] numbers = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
            }
        }
        else
        {
            //If a save file exists, read the grid layout from the file
            using (StreamReader reader = new StreamReader(filePathBoard))
            {
                int startLine = 2;
                string line;
                int currentLine = 1;
                int row = 0;

                while ((line = reader.ReadLine()) != null)
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
        
    }
    public void SaveBoard(Vector2Int position)
    {
        boardState = string.Empty;
        //Gets the entire board in the form of a string
        for (int y = 0; y < player.boardManager.gridLayout.GetLength(0); y++)
        {
            for (int x = 0; x < player.boardManager.gridLayout.GetLength(1); x++)
            {
                boardState += player.boardManager.gridLayout[y, x].ToString();
                if (x < player.boardManager.gridLayout.GetLength(1) - 1)
                {
                    boardState += " ";
                }
            }
            if (y < player.boardManager.gridLayout.GetLength(0) - 1)
            {
                boardState += "\n";
            }
        }

        //Saves to PlayerPrefs
        PlayerPrefs.SetString("boardState", boardState);
        PlayerPrefs.Save();

        //Saves to the save file as well
        //string emptyString = "";
        File.WriteAllText(filePathPlayer, string.Empty);
        File.AppendAllText(filePathPlayer, SceneManager.GetActiveScene().name + "\n");
        File.AppendAllText(filePathPlayer, player.gemCount.ToString() + "\n");
        File.AppendAllText(filePathPlayer, player.turnManager.quota.ToString() + "\n");

        File.WriteAllText(filePathBoard, string.Empty);
        File.AppendAllText(filePathBoard, player.currentPosition.ToString() + "\n");
        File.AppendAllText(filePathBoard, boardState + "\n");
    }

    /*public void NewGame()
    {
        //Checks to see if the save file exists, and if not creates it
        filePathPlayer = Application.persistentDataPath + "/PlayerData.txt";
        filePathBoard = Application.persistentDataPath + "/LevelData.txt";

        if (!File.Exists(filePathPlayer))
        {
            using (FileStream fs = File.Create(filePathPlayer))
            {

            }
        }

        if (!File.Exists(filePathBoard))
        {
            using (FileStream fs = File.Create(filePathBoard))
            {

            }
        }
    }*/
}
