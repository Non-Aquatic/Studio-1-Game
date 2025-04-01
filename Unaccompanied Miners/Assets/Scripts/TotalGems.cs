using UnityEngine;

public static class TotalGems
{
    private static int currency;

    static TotalGems()
    {
        currency = PlayerPrefs.GetInt("Currency", 0);
    }

    public static void ChangeTotalGems(int amount)
    {
        currency += amount;
        PlayerPrefs.SetInt("Currency", currency); 
        PlayerPrefs.Save();
    }

    public static int GetTotalGems()
    {
        return currency;
    }

    public static void ResetTotalGems()
    {
        currency = 0; 
        PlayerPrefs.SetInt("Currency", 0);  
        PlayerPrefs.Save();  
    }
}