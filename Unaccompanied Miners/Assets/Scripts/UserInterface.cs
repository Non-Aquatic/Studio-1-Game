using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using Unity.VisualScripting;

public class UserInterface : MonoBehaviour
{
    public BoardManager board; //Reference to board manager

    public TMP_Text quotaText; //Text to display the quota 
    int quota = 1; //Initial quota value
    int playerHealth = 0; //Player health
    public TMP_Text gems; //Text to show the number of gems the player has collected
    public TMP_Text knifes; 
    public TMP_Text level; //Text to show the current level's name
    public TMP_Text winText; //Text to show when the player wins
    //public TMP_Text loseText;
    public TMP_Text escapeText; //Text to show when the player escapes
    public GameObject player; //Reference to the player GameObject
    private Player playerScript; //Reference to the Player script
    private String sceneName; //Current scene name

    //UI elements for tutorial 1
    public GameObject tutShade;
    public GameObject tutPanel;
    public GameObject minePanel;
    public GameObject escapePanel;
    public GameObject goalPanel;
    public Button closeTutButton;
    public Button closeMineButton;
    public Button closeEscButton;
    public Button closeGoalPanel;

    //UI elements for tutorial 2
    public GameObject introGoblinPanel;
    public GameObject knifePanel;
    public GameObject shopPanel;
    public GameObject escPanel2;
    public Button closeIntroGob;
    public Button closeKnife;
    public Button closeShop;
    public Button closeEsc2;

    //UI elements for the pause menu
    public GameObject pausePanel;
    public GameObject pauseBackground;
    public Button restartButton;
    public Button saveGameButton;
    public Button shopButton;
    public Button mainMenuButton;
    public Button exitGameButton;
    private bool isPaused = false;
    //public GameObject ui;

    //UI elements for the quit menu
    public GameObject quitConfirmPanel;
    public Button resumeButton;
    public Button quitButton;

    //UI elements for the lose menu
    public GameObject youLosePanel;
    public Button retryButton;
    public Button menuButton;
    bool hasDied = false; //Bool to check if the player has died

    string folderPath; //Path to the save file
    string filePathPlayer;
    string filePathBoard;
    private SaveLoadScript saveScript;

    // Start is called before the first frame update
    void Start()
    {
        //ui.SetActive(false);
        //Invoke("LoadUI", 5);

        //Sets it so player has not died
        playerScript = player.GetComponent<Player>();
        saveScript = player.GetComponent<SaveLoadScript>();
        hasDied = false;

        //Initializes gems text to 0 and sets the level name
        gems.text = " 0";
        level.text = SceneManager.GetActiveScene().name;

        sceneName = SceneManager.GetActiveScene().name;
        //Sets correct quota for level
        SetQuota();
        quotaText.text = "Quota: " + quota.ToString();

        //Shows tutorial if it is level 1
        if(SceneManager.GetActiveScene().name == "Tutorial 1")
        {
            tutPanel.SetActive(true);
            //tutShade.SetActive(true);
        }

        if (SceneManager.GetActiveScene().name == "Tutorial 2")
        {
            introGoblinPanel.SetActive(true); 
        }

        //Sets several UI elements inactive
        pausePanel.SetActive(false);
        pauseBackground.SetActive(false);
        winText.gameObject.SetActive(false);
        minePanel.SetActive(false);
        escapePanel.SetActive(false);
        //loseText.gameObject.SetActive(false);
        escapeText.gameObject.SetActive(false);
        quitConfirmPanel.SetActive(false);
        youLosePanel.SetActive(false);
        // Add listeners for all the UI buttons
        closeTutButton.onClick.AddListener(CloseTutorial);
        closeMineButton.onClick.AddListener(CloseMine);
        closeEscButton.onClick.AddListener(CloseEsc);
        closeGoalPanel.onClick.AddListener(CloseGoal);
        closeIntroGob.onClick.AddListener(CloseIntro);
        closeKnife.onClick.AddListener(CloseKnife);
        closeShop.onClick.AddListener(CloseShop);
        closeEsc2.onClick.AddListener(CloseEsc2);
        restartButton.onClick.AddListener(RestartLevel);
        saveGameButton.onClick.AddListener(SaveGame);
        shopButton.onClick.AddListener(Shop);
        mainMenuButton.onClick.AddListener(ReturnToMenu);
        exitGameButton.onClick.AddListener(EnableQuitConfirm);
        quitButton.onClick.AddListener(ExitGame);
        resumeButton.onClick.AddListener(EnableMenu);
        retryButton.onClick.AddListener(RestartLevel);
        menuButton.onClick.AddListener(ReturnToMenu);

        //Sets up path to the save file
        folderPath = Path.Combine(Application.persistentDataPath, "GameData");
        filePathPlayer = Path.Combine(folderPath, "PlayerData.txt");
        filePathBoard = Path.Combine(folderPath, "LevelData.txt");
    }

    // Update is called once per frame
    void Update()
    {
        //Updates gems text 
        gems.text = " " + playerScript.gemCount.ToString();
        knifes.text = " " +  Items.LoadItemData("Knife");
        //Tracks health and triggers game over if health reaches 0
        playerHealth = playerScript.health;
        if(playerScript.health <= 0)
        {
            PlayerDied();
        }
        
        //Handles both pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    //Pauses the game
    void PauseGame()
    {
        //Sets pause menu to active
        pausePanel.SetActive(true);
        pauseBackground.SetActive(true);
        //Stops game movement
        Time.timeScale = 0f;
        //Disables player input
        playerScript.enabled = false;
        //Sets pause bool to true
        isPaused = true;
    }
    //Unpauses the game
    void ResumeGame()
    {
        //Sets pause menu to inactive
        pausePanel.SetActive(false);
        pauseBackground.SetActive(false);
        //Resumes game movement
        Time.timeScale = 1f;
        //Enables player input
        playerScript.enabled = true;
        //Sets pause bool to false
        isPaused = false;
    }
    //Restarts the level
    void RestartLevel()
    {
        //If player has died, clears save file
        if (hasDied)
        {
            string emptyString = "";
            File.WriteAllText(filePathPlayer, emptyString);
            File.WriteAllText(filePathBoard, emptyString);
        }
        //Makes sure game movment is running as normal
        Time.timeScale = 1f; 
        //Reloads current scene
        SceneManager.LoadScene(level.text);
    }
    //Saves the game
    void SaveGame()
    {
        //Saves the gem count and quota to PlayerPrefs
        PlayerPrefs.SetInt("GemsCount", playerScript.gemCount);
        PlayerPrefs.SetInt("Quota", quota);
        PlayerPrefs.Save();

        //Clears old save file
        string emptyString = "";
        File.WriteAllText(filePathPlayer, emptyString);
        File.WriteAllText(filePathBoard, emptyString);
        //Fills in new save file
        saveScript.SaveBoard(playerScript.currentPosition);
    }
    //Returns to main menu
    void ReturnToMenu()
    {
        //Makes sure game movment is running as normal
        Time.timeScale = 1f;
        //Sets pause bool to false
        isPaused = false;
        //Loads main menu
        SceneManager.LoadScene("Main Menu");
    }
    void Shop()
    {
        //Makes sure game movment is running as normal
        Time.timeScale = 1f;
        //Sets pause bool to false
        isPaused = false;
        //Loads main menu
        SceneManager.LoadScene("Shop");
    }
    //Exits game
    void ExitGame()
    {
        Application.Quit();
    }
    //Sets quota based on board manager
    void SetQuota()
    {
        quota = board.quota;
       
        
        /*if(sceneName == "Level 1")
        {
            board.quota = quota;
        }
        if(sceneName == "Level 2")
        {
            quota = 35;
        }*/
    }
    //Hnadles winning the game
    public void winGame()
    {
        Debug.Log("You Won");
        //Shows win text
        winText.gameObject.SetActive(true);
        //Disables player input
        playerScript.enabled = false;
        Debug.Log(sceneName);
        //If Tutorial 1, saves and loads level 1 after 10 seconds
        if (sceneName == "Tutorial 1")
        {
            string emptyString = "";
            File.WriteAllText(filePathPlayer, emptyString);
            File.WriteAllText(filePathBoard, emptyString);
            Invoke("LoadTutorial2", 10);
        }
        //If level 1, saves and loads level 2 after 10 seconds
        /* if (sceneName == "Level 1")
        {
            string emptyString = "";
            File.WriteAllText(filePathPlayer, emptyString);
            File.WriteAllText(filePathBoard, emptyString);
            Invoke("LoadLevel2", 10);
        }*/
        if (sceneName != "Tutorial 1")
        {
            string emptyString = "";
            File.WriteAllText(filePathPlayer, emptyString);
            File.WriteAllText(filePathBoard, emptyString);
            Invoke("LoadLevelSelect", 10);
        }
        //If level 2, goes back to main menu after 10 seconds
        if (sceneName != "Tutorial 1" || sceneName != "Tutorial 2")
        {
            string[] levelmax = sceneName.Split(' ');
            PlayerPrefs.SetInt("MaxLevelCompleted", Int32.Parse(levelmax[1]));
        }
    }
    //Handles escaping the game
    public void escapeGame()
    {
        Debug.Log("You Escaped");
        //Shows escape text
        escapeText.gameObject.SetActive(true);
        //Time.timeScale = 0f;
        //Loads main menu after 10 seconds
        Invoke("ReturnToMainMenu", 10);
    }

    //Loads level 1
    void LoadLevel1()
    {
        SceneManager.LoadScene("Level 1");
    }

    //Loads level 2
    void LoadLevel2()
    {
        SceneManager.LoadScene("Level 2");
    }
    void LoadTutorial2()
    {
        SceneManager.LoadScene("Tutorial 2");
    }
    void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    //Loads main menu
    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    //Enables pause menu
    void EnableMenu()
    {
        pausePanel.SetActive(true);
        quitConfirmPanel.SetActive(false);
    }
    //Enables quit confirmation screen
    void EnableQuitConfirm()
    {
        pausePanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }
    //Closes the tutorial
    void CloseTutorial()
    {
        tutPanel.SetActive(false);
        minePanel.SetActive(true);
    }

    void CloseMine()
    {
        minePanel.SetActive(false);
        escapePanel.SetActive(true);
    }

    void CloseEsc()
    {
        escapePanel.SetActive(false);
        goalPanel.SetActive(true);
    }

    void CloseGoal()
    {
        goalPanel.SetActive(false);
    }

    void CloseIntro()
    {
        introGoblinPanel.SetActive(false);
        knifePanel.SetActive(true);
    }

    void CloseKnife()
    {
        knifePanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    void CloseShop()
    {
        shopPanel.SetActive(false);
        escPanel2.SetActive(true);
    }

    void CloseEsc2()
    {
        escPanel2.SetActive(false);
    }

    //Handles player death
    void PlayerDied()
    {
        //Shows you lose screen
        youLosePanel.SetActive(true);
        pauseBackground.SetActive(true);
        //Disables player input
        playerScript.enabled = false;
        //Sets death bool to true
        hasDied = true;
    }
}
