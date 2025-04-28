using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Shop : MonoBehaviour
{
    public TMP_Text playerGemText;  
    public TMP_Text knifeText;
    public TMP_Text healthPotionText;
    public TMP_Text shieldText;
    public TMP_Text freedomPassText;

    public Button knifeButton;
    public Button healthPotionButton;
    public Button shieldButton;
    public Button freedomPassButton;
    public Button levelSelect;

    public int knifeCost = -10;
    public int healthPotionCost = -30;
    public int shieldCost = -20;
    public int freedomPassCost = -150;

    private bool testStop = true;

    private int playerGemCount;
    private void Update()
    {
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
        if (Items.LoadItemData("Freedom") > 0 && !testStop)
        {
            SceneManager.LoadScene("Finale");
        }
        playerGemCount = TotalGems.GetTotalGems();

        knifeButton.interactable = playerGemCount >= -knifeCost;
        healthPotionButton.interactable = playerGemCount >= -healthPotionCost;
        shieldButton.interactable = playerGemCount >= -shieldCost;
        freedomPassButton.interactable = playerGemCount >= -freedomPassCost;
        UpdateGemUI();
    }
    void Start()
    {
        playerGemCount = TotalGems.GetTotalGems();
        UpdateGemUI();

        knifeText.text = "Knife - " + -knifeCost;
        healthPotionText.text = "Potion - " + -healthPotionCost;
        shieldText.text = "Shield - " + -shieldCost;
        freedomPassText.text = "Freedom - " + -freedomPassCost;

        knifeButton.onClick.AddListener(BuyKnife);
        healthPotionButton.onClick.AddListener(BuyHealthPotion);
        shieldButton.onClick.AddListener(BuyShield);
        freedomPassButton.onClick.AddListener(BuyFreedom);
        levelSelect.onClick.AddListener(LevelSelect);
    }

    void UpdateGemUI()
    {
        playerGemCount = TotalGems.GetTotalGems();
        playerGemText.text = " " + playerGemCount.ToString();
    }

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
    void BuyHealthPotion()
    {
        if (playerGemCount >= -healthPotionCost)
        {
            TotalGems.ChangeTotalGems(healthPotionCost);
            Items.AddItem("Potion", 1);
            UpdateGemUI();
        }
    }

    void BuyShield()
    {
        if (playerGemCount >= -shieldCost)
        {
            TotalGems.ChangeTotalGems(shieldCost);
            Items.AddItem("Shield", 1);
            UpdateGemUI();
        }
    }

    void BuyFreedom()
    {
        if (playerGemCount >= -freedomPassCost)
        {
            TotalGems.ChangeTotalGems(freedomPassCost);
            Items.AddItem("Freedom", 1);
            UpdateGemUI();
        }
    }
    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
