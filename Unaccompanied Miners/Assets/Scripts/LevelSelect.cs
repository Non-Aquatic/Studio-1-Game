using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button l1Button;
    public Button l2Button;
    public Button l3Button;
    public Button l4Button;
    public Button mainMenuButton;
    public Button shopButton;

    void Start()
    {
        l4Button.onClick.AddListener(GoToL4);
        l3Button.onClick.AddListener(GoToL3);
        l1Button.onClick.AddListener(GoToL1);
        l2Button.onClick.AddListener(GoToL2);
        mainMenuButton.onClick.AddListener(ReturnToMenu);
        shopButton.onClick.AddListener(Shop);
        CheckLevelProgress();

    }
    void CheckLevelProgress()
    {

        int maxLevelCompleted = PlayerPrefs.GetInt("MaxLevelCompleted", 0);

        l1Button.interactable = true;
        l2Button.interactable = (maxLevelCompleted >= 1);
        l3Button.interactable = (maxLevelCompleted >= 2);
        l4Button.interactable = (maxLevelCompleted >= 3);
    }
    void GoToL1()
    {
        SceneManager.LoadScene("Level 1");
    }

    void GoToL2()
    {
        SceneManager.LoadScene("Level 2");
    }
    void GoToL3()
    {
        SceneManager.LoadScene("Level 3");
    }

    void GoToL4()
    {
        SceneManager.LoadScene("Level 4");
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    void Shop()
    {
        SceneManager.LoadScene("Shop");
    }
}
