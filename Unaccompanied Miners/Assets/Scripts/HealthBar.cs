using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public Player playerHealth;
    public Image healthBarColor;
    // Start is called before the first frame update
    void Start()
    {
        //playerHealth = GetComponent<Player>();
        //healthBar = GetComponent<Slider>();
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int hp)
    {
        healthBar.value = hp;
        if(healthBar.value <= 3) 
        {
            Color color = new Color(1f, 0f, 0f);
            healthBarColor.color = color;
            
        }
    }
}
