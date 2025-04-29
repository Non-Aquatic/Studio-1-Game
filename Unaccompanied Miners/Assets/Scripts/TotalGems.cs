using UnityEngine;

//Script to keep track of the total number of gems the player currently has
public static class TotalGems
{
    //Int for gems
    private static int currency;

    //Loads total gems using playerPrefs
    static TotalGems()
    {
        currency = PlayerPrefs.GetInt("Currency", 0);
    }
    //Adds or subtracts an amount of gems from the total
    public static void ChangeTotalGems(int amount)
    {
        currency += amount;
        PlayerPrefs.SetInt("Currency", currency); 
        PlayerPrefs.Save();
    }
    //Getter for total gems
    public static int GetTotalGems()
    {
        return currency;
    }
    //Complete resets total gems for new game
    public static void ResetTotalGems()
    {
        currency = 0; 
        PlayerPrefs.SetInt("Currency", 0);  
        PlayerPrefs.Save();  
    }
}