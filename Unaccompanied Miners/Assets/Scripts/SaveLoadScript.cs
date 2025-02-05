using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Transactions;

public class SaveLoadScript : MonoBehaviour
{
    public Player player;
    public string boardState = ""; //Board state
    string filePath; //Path to the save file

    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/saveData.txt";
        player = GetComponent<Player>();

        //Reads saved gem count from file if avaliable
        string line2 = ReadLine(filePath, 2);
        if (line2 != null)
        {
            player.gemCount = int.Parse(line2);
        }

        //Reads saved player position from file if avaliable
        string line4 = ReadLine(filePath, 4);
        //If position is not saved, start at (0, 0)
        if (line4 == null)
        {
            player.currentPosition = new Vector2Int(0, 0);
            player.targetPosition = transform.position;
        }
        //If position is saved, places player at that position
        if (line4 != null)
        {
            string removeParentheses = line4.Replace("(", "").Replace(")", "").Trim();
            string[] parts = removeParentheses.Split(',');
            int x = int.Parse(parts[0].Trim());
            int y = int.Parse(parts[1].Trim());
            player.currentPosition = new Vector2Int(x, y);
            player.targetPosition = new Vector3(player.currentPosition.x, 1f, player.currentPosition.y);
            transform.position = player.targetPosition;
        }

        SaveBoard(player.currentPosition);
    }

    // Update is called once per frame
    void Update()
    {

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
        string emptyString = "";
        File.WriteAllText(filePath, string.Empty);
        File.AppendAllText(filePath, SceneManager.GetActiveScene().name + "\n");
        File.AppendAllText(filePath, player.gemCount.ToString() + "\n");
        File.AppendAllText(filePath, player.turnManager.quota.ToString() + "\n");
        File.AppendAllText(filePath, player.currentPosition.ToString() + "\n");
        File.AppendAllText(filePath, boardState + "\n");
    }
}
