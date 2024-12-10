using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar; //Reference to slider that is the health bar
    public Player playerHealth; //Player reference to get health left
    public Image healthBarColor; // Image to visually see health bar

    void Start()
    {
        //playerHealth = GetComponent<Player>();
        //healthBar = GetComponent<Slider>();

        //Initially sets them at max
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.maxHealth;
    }
    //Updates health when the player takes damage
    public void SetHealth(int hp)
    {
        healthBar.value = hp;
        //If below 3 health, makes the health bar red
        if(healthBar.value <= 3) 
        {
            Color color = new Color(1f, 0f, 0f);
            healthBarColor.color = color;
            
        }
    }
}
