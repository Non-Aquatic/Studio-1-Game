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
    string firstLine = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        loadGameButton.onClick.AddListener(WaitForMovement);
        exitButton.onClick.AddListener(ExitGame);

        filePath = Application.persistentDataPath + "/saveData.txt";
        if (!File.Exists(filePath))
        {
            using(FileStream fs = File.Create(filePath))
            {

            }
        }

        
        using (StreamReader sr = new StreamReader(filePath))
        {
            firstLine = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(firstLine))
            {
                loadGameButton.interactable = false;
            }
            else
            {
                loadGameButton.interactable = true;
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

    void WaitForMovement()
    {
        if (firstLine == "Level 1" || firstLine == "Level 2")
        {
            //SceneManager.LoadScene(firstLine, LoadSceneMode.Additive);
            Invoke("LoadGame", 1);
        }
        //Debug.Log("Until I get to the Save Game feature, this will just load the first level.  - Mahliq");
    }

    void LoadGame()
    {
        //SceneManager.UnloadSceneAsync("Main Menu");
        SceneManager.LoadScene(firstLine);
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
