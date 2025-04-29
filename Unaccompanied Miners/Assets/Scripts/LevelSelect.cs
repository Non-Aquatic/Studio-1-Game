using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    //References to all 4 level buttons, the main menu button, and the shop button
    public Button l1Button;
    public Button l2Button;
    public Button l3Button;
    public Button l4Button;
    public Button mainMenuButton;
    public Button shopButton;

    //Adds listeners to all buttons as sees the highest level the player has beaten
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
    //Checks the highest level the player has beaten and unlocks all up to +1 of that leevl
    //For example: If the player has beaten level 2 then levels 1,2,3 are unlocked but not level 4
    void CheckLevelProgress()
    {

        int maxLevelCompleted = PlayerPrefs.GetInt("MaxLevelCompleted", 0);

        l1Button.interactable = true;
        l2Button.interactable = (maxLevelCompleted >= 1);
        l3Button.interactable = (maxLevelCompleted >= 2);
        l4Button.interactable = (maxLevelCompleted >= 3);
    }
    //Methods to travel to all levels
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
    //Method to go to main menu
    void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    //Method to go to the shop
    void Shop()
    {
        SceneManager.LoadScene("Shop");
    }
}
