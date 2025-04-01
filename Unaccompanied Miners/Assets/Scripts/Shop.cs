using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public TMP_Text playerGemText;  
    public TMP_Text shopItemText;   
    public Button buyButton;
    public Button mainMenu;
    public int knifeCost = -10;     

    private int playerGemCount;
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                TotalGems.ResetTotalGems();
                Items.SaveItemData("Knife", 1);
                UpdateGemUI();
            }
        }
    }
    void Start()
    {
        playerGemCount = TotalGems.GetTotalGems();
        UpdateGemUI();

        shopItemText.text = "Knife - " + -knifeCost;

        buyButton.onClick.AddListener(BuyKnife);
        mainMenu.onClick.AddListener(MainMenu);
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
    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
