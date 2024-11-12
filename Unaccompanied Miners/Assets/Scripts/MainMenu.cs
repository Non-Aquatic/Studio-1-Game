using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Button newGameButton;
    public Button loadGameButton;
    public Button exitButton;
    public GameObject OptionsPanel;
    bool optionsPanelOpen = false;

    string filePath;

    // Start is called before the first frame update
    void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        loadGameButton.onClick.AddListener(LoadGame);
        exitButton.onClick.AddListener(ExitGame);

        filePath = Application.persistentDataPath + "/saveData.txt";
        if (!File.Exists(filePath))
        {
            using(FileStream fs = File.Create(filePath))
            {

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NewGame()
    {
        string emptyString = "";
        File.WriteAllText(filePath, emptyString);
        SceneManager.LoadScene("Level 1");
    }

    void LoadGame()
    {
        //Until I get to the Save Game feature, this will just load the first level.  - Mahliq
        SceneManager.LoadScene("Level 1");
        Debug.Log("Until I get to the Save Game feature, this will just load the first level.  - Mahliq");
    }

    public void ToggleOptions()
    {
        optionsPanelOpen = !optionsPanelOpen;
        OptionsPanel.SetActive(optionsPanelOpen);
    }

    void ExitGame()
    {
        Application.Quit();
    }
}
