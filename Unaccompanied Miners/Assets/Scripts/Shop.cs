using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Shop : MonoBehaviour
{
    //References to all the text in the shop
    public TMP_Text playerGemText;  
    public TMP_Text knifeText;
    public TMP_Text healthPotionText;
    public TMP_Text shieldText;
    public TMP_Text freedomPassText;

    //References to all the buttons in the shop
    public Button knifeButton;
    public Button healthPotionButton;
    public Button shieldButton;
    public Button freedomPassButton;
    public Button levelSelect;

    //Prices of all the items
    public int knifeCost = -10;
    public int healthPotionCost = -30;
    public int shieldCost = -20;
    public int freedomPassCost = -150;

    //Debug value for testing finale and shop
    private bool testStop = false;

    //Player current gem count 
    private int playerGemCount;
    private void Update()
    {
        //Input to reset all items and gems (for testing)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                TotalGems.ResetTotalGems();
                PlayerPrefs.SetInt("MaxLevelCompleted", 0);
                Items.SaveItemData("Knife", 1);
                Items.SaveItemData("Potion", 0);
                Items.SaveItemData("Shield", 0);
                Items.SaveItemData("Freedom", 0);
                UpdateGemUI();
            }
        }
        //Input to get a lot of gems (for testing)
        if (Input.GetKey(KeyCode.I))
        {
            if (Input.GetKey(KeyCode.U))
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    TotalGems.ChangeTotalGems(200);
                }
            }
        }
        //If the player has bought thier freedom, it takes the player to the finale
        if (Items.LoadItemData("Freedom") > 0 && !testStop)
        {
            SceneManager.LoadScene("Finale");
        }
        //Checks the amount of gems the player currently has
        playerGemCount = TotalGems.GetTotalGems();

        //Makes the item buttons uninteractable if you don't have enough gems to buy it
        knifeButton.interactable = playerGemCount >= -knifeCost;
        healthPotionButton.interactable = playerGemCount >= -healthPotionCost;
        shieldButton.interactable = playerGemCount >= -shieldCost;
        freedomPassButton.interactable = playerGemCount >= -freedomPassCost;
        //Updates gems the player has
        UpdateGemUI();
    }
    void Start()
    {
        //Gathers how many gems the player has and displays it
        playerGemCount = TotalGems.GetTotalGems();
        UpdateGemUI();
        
        //Displays price text for all items
        knifeText.text = "Knife - " + -knifeCost;
        healthPotionText.text = "Potion - " + -healthPotionCost;
        shieldText.text = "Shield - " + -shieldCost;
        freedomPassText.text = "Freedom - " + -freedomPassCost;

        //Adds listeners to all item buttons
        knifeButton.onClick.AddListener(BuyKnife);
        healthPotionButton.onClick.AddListener(BuyHealthPotion);
        shieldButton.onClick.AddListener(BuyShield);
        freedomPassButton.onClick.AddListener(BuyFreedom);
        levelSelect.onClick.AddListener(LevelSelect);
    }
    //Updates how many gems the player currently has
    void UpdateGemUI()
    {
        playerGemCount = TotalGems.GetTotalGems();
        playerGemText.text = " " + playerGemCount.ToString();
    }
    //If the player has enough gems, it buys them a knife
    void BuyKnife()
    {
        if (playerGemCount >= -knifeCost)
        {
            TotalGems.ChangeTotalGems(knifeCost);
            playerGemCount = TotalGems.GetTotalGems();
            Items.AddItem("Knife", 1);  
            UpdateGemUI();
        }
    }
    //If the player has enough gems, it buys them a potion
    void BuyHealthPotion()
    {
        if (playerGemCount >= -healthPotionCost)
        {
            TotalGems.ChangeTotalGems(healthPotionCost);
            Items.AddItem("Potion", 1);
            UpdateGemUI();
        }
    }
    //If the player has enough gems, it buys them a shield
    void BuyShield()
    {
        if (playerGemCount >= -shieldCost)
        {
            TotalGems.ChangeTotalGems(shieldCost);
            Items.AddItem("Shield", 1);
            UpdateGemUI();
        }
    }
    //If the player has enough gems, it buys their debt off and wins the game
    void BuyFreedom()
    {
        if (playerGemCount >= -freedomPassCost)
        {
            TotalGems.ChangeTotalGems(freedomPassCost);
            Items.AddItem("Freedom", 1);
            UpdateGemUI();
        }
    }
    //Allows the player to go to the level select
    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
