using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button newGameButton;
    public Button loadGameButton;
    public Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        loadGameButton.onClick.AddListener(LoadGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NewGame()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
        SceneManager.LoadScene("Camera and Lights", LoadSceneMode.Additive);
    }

    void LoadGame()
    {
        //Until I get to the Save Game feature, this will just load the first level.  - Mahliq
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
        SceneManager.LoadScene("Camera and Lights", LoadSceneMode.Additive);
        Debug.Log("Until I get to the Save Game feature, this will just load the first level.  - Mahliq");
    }

    void ExitGame()
    {
        Application.Quit();
    }
}
