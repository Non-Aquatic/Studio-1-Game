using UnityEngine;

//Class script for the items
public static class Items
{
    //Set the items to be a specific value
    public static void SaveItemData(string itemName, int quantity)
    {
        PlayerPrefs.SetInt(itemName, quantity);
        PlayerPrefs.Save();
    }
    //Returns how many of that item the player currently owns
    public static int LoadItemData(string itemName)
    {
        return PlayerPrefs.GetInt(itemName, 1);
    }
    //Adds a specific amount of the item to the players inventory (usually 1)
    public static void AddItem(string itemName, int amount)
    {
        int currentAmount = LoadItemData(itemName);
        SaveItemData(itemName, currentAmount + amount);
    }
    //Gets rid of a single one of a specific item, like the player is using it
    public static bool UseItem(string itemName)
    {
        int currentAmount = LoadItemData(itemName);
        if (currentAmount > 0)
        {
            SaveItemData(itemName, currentAmount - 1);
            return true; 
        }
        return false;
    }
}