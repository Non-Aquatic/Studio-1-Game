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
    public BoardManager board;

    public TMP_Text quotaText;
    int quota = 1;
    public TMP_Text gems;
    public TMP_Text level;
    public TMP_Text winText;
    public TMP_Text loseText;
    public TMP_Text escapeText;
    public GameObject player;
    private Player playerScript;
    private String sceneName;

    public GameObject tutShade;
    public GameObject tutPanel;
    public Button closeTutButton;

    public GameObject pausePanel;
    public GameObject pauseBackground;
    public Button restartButton;
    public Button saveGameButton;
    public Button mainMenuButton;
    public Button exitGameButton;
    private bool isPaused = false;
    //public GameObject ui;

    public GameObject quitConfirmPanel;
    public Button resumeButton;
    public Button quitButton;

    string filePath;

    // Start is called before the first frame update
    void Start()
    {
        //ui.SetActive(false);
        //Invoke("LoadUI", 5);

        playerScript = player.GetComponent<Player>();

        gems.text = " 0";
        level.text = SceneManager.GetActiveScene().name;

        sceneName = SceneManager.GetActiveScene().name;
        SetQuota();
        quotaText.text = "Quota: " + quota.ToString();

        if(SceneManager.GetActiveScene().name == "Level 1")
        {
            tutPanel.SetActive(true);
            tutShade.SetActive(true);
        }

        pausePanel.SetActive(false);
        pauseBackground.SetActive(false);
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
        escapeText.gameObject.SetActive(false);
        quitConfirmPanel.SetActive(false);
        closeTutButton.onClick.AddListener(CloseTutorial);
        restartButton.onClick.AddListener(RestartLevel);
        saveGameButton.onClick.AddListener(SaveGame);
        mainMenuButton.onClick.AddListener(ReturnToMenu);
        exitGameButton.onClick.AddListener(EnableQuitConfirm);
        quitButton.onClick.AddListener(ExitGame);
        resumeButton.onClick.AddListener(EnableMenu);

        filePath = Application.persistentDataPath + "/saveData.txt";
    }

    // Update is called once per frame
    void Update()
    { 
        if(playerScript.gemCount >= quota) //MOVE THIS SECTION TO TURN MANNAGER 
        {
            winText.gameObject.SetActive(true);
            playerScript.enabled = false;
        }
        gems.text = " " + playerScript.gemCount.ToString();
        
        
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

    void PauseGame()
    {
        pausePanel.SetActive(true);
        pauseBackground.SetActive(true);
        Time.timeScale = 0f;
        playerScript.enabled = false;
        isPaused = true;
    }

    void ResumeGame()
    {
        pausePanel.SetActive(false);
        pauseBackground.SetActive(false);
        Time.timeScale = 1f;
        playerScript.enabled = true;
        isPaused = false;
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(level.text);
    }

    void SaveGame()
    {
        //I'm not sure how we want to go about doing this so this is more for show atm.
        PlayerPrefs.SetInt("GemsCount", playerScript.gemCount);
        PlayerPrefs.SetInt("Quota", quota);
        PlayerPrefs.Save();

        string emptyString = "";
        File.WriteAllText(filePath, emptyString);
        File.AppendAllText(filePath, level.text + "\n");
        File.AppendAllText(filePath, playerScript.gemCount.ToString() + "\n");
        File.AppendAllText(filePath, quota.ToString() + "\n");
        File.AppendAllText(filePath, playerScript.currentPosition.ToString() + "\n");
        File.AppendAllText(filePath, playerScript.boardState + "\n");
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("Main Menu");
    }

    void ExitGame()
    {
        Application.Quit();
    }

    void SetQuota()
    {
        quota = board.quota;
       
        /*
        if(sceneName == "Level 1")
        {
            quota = 25;
        }
        if(sceneName == "Level 2")
        {
            quota = 35;
        }*/
    }

    public void winGame()
    {
        Debug.Log("You Won");
        winText.gameObject.SetActive(true);
        //Time.timeScale = 0f;
        Invoke("ReturnToMainMenu", 10);
    }

    public void escapeGame()
    {
        Debug.Log("You Escaped");
        escapeText.gameObject.SetActive(true);
        //Time.timeScale = 0f;
        Invoke("ReturnToMainMenu", 10);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    void LoadUI()
    {
        //ui.SetActive(true);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelSceneName));
    }

    void EnableMenu()
    {
        pausePanel.SetActive(true);
        quitConfirmPanel.SetActive(false);
    }

    void EnableQuitConfirm()
    {
        pausePanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    void CloseTutorial()
    {
        tutPanel.SetActive(false);
        tutShade.SetActive(false);
    }
}
