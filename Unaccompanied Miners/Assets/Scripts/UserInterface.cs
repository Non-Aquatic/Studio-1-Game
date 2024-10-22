using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

public class UserInterface : MonoBehaviour
{
    public TMP_Text quotaText;
    public int quota = 1;
    public TMP_Text gems;
    public TMP_Text level;
    public TMP_Text winText;
    public GameObject player;
    private Player playerScript;
    private String sceneName;

    public GameObject pausePanel;
    public GameObject pauseBackground;
    public Button restartButton;
    public Button saveGameButton;
    public Button mainMenuButton;
    public Button exitGameButton;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<Player>();

        gems.text = "Gems: 0";
        level.text = SceneManager.GetActiveScene().name;

        sceneName = SceneManager.GetActiveScene().name;
        SetQuota();
        quotaText.text = "Quota: " + quota.ToString();

        pausePanel.SetActive(false);
        pauseBackground.SetActive(false);
        winText.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartLevel);
        saveGameButton.onClick.AddListener(SaveGame);
        mainMenuButton.onClick.AddListener(ReturnToMenu);
        exitGameButton.onClick.AddListener(ExitGame);
    }

    // Update is called once per frame
    void Update()
    { 
        if(playerScript.gemCount > quota)
        {
            winText.gameObject.SetActive(true);
        }
        gems.text = "Gems: " + playerScript.gemCount.ToString();
        
        
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
        SceneManager.LoadScene(level.text);
    }

    void SaveGame()
    {
        //I'm not sure how we want to go about doing this so this is more for show atm.
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    void ExitGame()
    {
        Application.Quit();
    }

    void SetQuota()
    {
        if(sceneName == "Level 1")
        {
            quota = 25;
        }
        if(sceneName == "Level 2")
        {
            quota = 35;
        }
    }

    public void winGame()
    {
        Debug.Log("You Won");
        winText.gameObject.SetActive(true);
        //Time.timeScale = 0f;
        Invoke("ReturnToMainMenu", 10);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
