using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    public TMP_Text quota;
    public TMP_Text gems;
    public TMP_Text level;
    public GameObject player;
    private Player playerScript;

    public GameObject pausePanel;
    public Button restartButton;
    public Button saveGameButton;
    public Button mainMenuButton;
    public Button exitGameButton;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<Player>();

        gems.text = "Gems: 0";
        level.text = SceneManager.GetActiveScene().name;

        pausePanel.SetActive(false);
        restartButton.onClick.AddListener(RestartLevel);
        saveGameButton.onClick.AddListener(SaveGame);
        mainMenuButton.onClick.AddListener(ReturnToMenu);
        exitGameButton.onClick.AddListener(ExitGame);
    }

    // Update is called once per frame
    void Update()
    { 
        gems.text = "Gems: " + playerScript.gemCount.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(level.text);
    }

    void SaveGame()
    {

    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    void ExitGame()
    {
        Application.Quit();
    }
}
