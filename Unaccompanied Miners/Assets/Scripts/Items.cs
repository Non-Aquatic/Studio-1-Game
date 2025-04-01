using UnityEngine;

public static class Items
{
    public static void SaveItemData(string itemName, int quantity)
    {
        PlayerPrefs.SetInt(itemName, quantity);
        PlayerPrefs.Save();
    }

    public static int LoadItemData(string itemName)
    {
        return PlayerPrefs.GetInt(itemName, 1);
    }

    public static void AddItem(string itemName, int amount)
    {
        int currentAmount = LoadItemData(itemName);
        SaveItemData(itemName, currentAmount + amount);
    }

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