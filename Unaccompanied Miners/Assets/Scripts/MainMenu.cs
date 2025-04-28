using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

public class MainMenu : MonoBehaviour
{
    // UI buttons for the main menu
    public Button newGameButton; //Button that starts a new game
    public Button loadGameButton; //Button that loads a previous game
    public Button exitButton; //Button to exit application

    public GameObject OptionsPanel; //Panel for options/settings
    bool optionsPanelOpen = false; //Bool to check whether setting are open

    string folderPath;
    string filePathPlayer; //Path to the save file
    string filePathBoard;
    string firstLine = string.Empty; //First line from save file

    void Start()
    {
        //Add listeners for all the main menu buttons
        newGameButton.onClick.AddListener(NewGame);
        loadGameButton.onClick.AddListener(WaitForMovement);
        exitButton.onClick.AddListener(ExitGame);
        //Set screen resolution to 1920x1080 in fullscreen mode
        Screen.SetResolution(1920, 1080, true);

        //Checks to see if the save file exists, and if not creates it
        folderPath = Path.Combine(Application.persistentDataPath, "GameData");
        filePathPlayer = Path.Combine(folderPath, "PlayerData.txt");
        filePathBoard = Path.Combine(folderPath, "LevelData.txt");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath); 
        }
        if (!File.Exists(filePathPlayer))
        {
            using(FileStream fs = File.Create(filePathPlayer))
            {

            }
        }

        if (!File.Exists(filePathBoard))
        {
            using (FileStream fs = File.Create(filePathBoard))
            {

            }
        }

        // Read the first line from the save file to see whether you can load the game
        using (StreamReader sr = new StreamReader(filePathPlayer))
        {
            firstLine = sr.ReadLine();
            // If the save file is empty, disable the Load Game button
            if (string.IsNullOrWhiteSpace(firstLine))
            {
                loadGameButton.interactable = false;
            }
            //If the save file is not empty, eneable the Load Game button
            else
            {
                loadGameButton.interactable = true;
            }
        }
    }

    //Starts a new game
    void NewGame()
    {
        //Clears save file and loads first level
        string emptyString = "";
        string currentScene = "Tutorial 1";
        File.WriteAllText(filePathPlayer, emptyString);
        File.WriteAllText(filePathBoard, emptyString);
        PlayerPrefs.SetString(currentScene, currentScene);
        TotalGems.ResetTotalGems();
        Items.SaveItemData("Knife", 1);
        PlayerPrefs.SetInt("MaxLevelCompleted", 0);
        Items.SaveItemData("Potion", 0);
        Items.SaveItemData("Shield", 0);
        Items.SaveItemData("Freedom", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Tutorial 1");
    }

    // Waits for movement before loading the game
    void WaitForMovement()
    {
       //Makes sure it is a valid level
        if (firstLine == "Level 1" || firstLine == "Level 2" || firstLine == "Tutorial 1")
        {
            //Loads specified level after 1 second
            Invoke("LoadGame", 1);
        }
    }

    //Loads a saved game
    void LoadGame()
    {
        //Looks at first line and loads corresponding level
        SceneManager.LoadScene(firstLine);
    }

    //Changes visibility of options menu
    public void ToggleOptions()
    {
        optionsPanelOpen = !optionsPanelOpen;
        OptionsPanel.SetActive(optionsPanelOpen);
    }
    //Exits the application
    void ExitGame()
    {
        Application.Quit();
    }
}
