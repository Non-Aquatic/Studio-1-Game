using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watchtower1 : MonoBehaviour
{
    //Different positions for goblins on watchtower
    public GameObject Goblin0;
    public GameObject Goblin1;
    public GameObject Goblin2;
    public GameObject Goblin3;

    //Different positions for spotligh
    public GameObject Spotlight0;
    public GameObject Spotlight1;
    public GameObject Spotlight2;
    public GameObject Spotlight3;

    //Current spotlight position
    public int currentSpot;
    //Current player position
    public GameObject PlayerPosition;
    //Track whether player has taken damage from spolight yet
    private bool[] hasPlayerTakenDamage = new bool[4] { false, false, false, false };
    //Previous player position
    private Vector2Int previousPlayerPosition;

    public void Start()
    {
        //Sets all goblin and spotlight positions to be inactive
        Goblin3.SetActive(false);
        Spotlight3.SetActive(false);
        Goblin0.SetActive(false);
        Spotlight0.SetActive(false);
        Goblin1.SetActive(false);
        Spotlight1.SetActive(false);
        Goblin2.SetActive(false);
        Spotlight2.SetActive(false);

        //Switches to initial spot based on current spot
        switch (currentSpot)
        {
            case 0:
                SwapTo0();
                break;
            case 1:
                SwapTo1();
                break;
            case 2:
                SwapTo2();
                break;
            case 3:
                SwapTo3();
                break;
        }
    }
    //Cycles between the different spots (Goblin and spotlight)
    public void SwapPosition()
    {
        //0 to 1
        if (currentSpot == 0)
        {
            SwapTo1();
        }
        //1 to 2
        else if (currentSpot == 1)
        {
            SwapTo2();
        }
        //2 to 3
        else if (currentSpot == 2)
        {
            SwapTo3();
        }
        //3 to 0
        else if (currentSpot == 3)
        {
            SwapTo0();
        }
    }
    //Switches to spot 0 for goblin and spotlight
    public void SwapTo0()
    {
        //Deactivates goblin and spotlight 3
        Goblin3.SetActive(false);
        Spotlight3.SetActive(false);
        //Activates goblin and spotlight 0
        Goblin0.SetActive(true);
        Spotlight0.SetActive(true);
        //Sets current position to 0
        currentSpot = 0;
    }
    //Switches to spot 1 for goblin and spotlight
    public void SwapTo1()
    {
        //Deactivates goblin and spotlight 0
        Goblin0.SetActive(false);
        Spotlight0.SetActive(false);
        //Activates goblin and spotlight 1
        Goblin1.SetActive(true);
        Spotlight1.SetActive(true);
        //Sets current position to 1
        currentSpot = 1;
    }
    //Switches to spot 2 for goblin and spotlight
    public void SwapTo2()
    {
        //Deactivates goblin and spotlight 1
        Goblin1.SetActive(false);
        Spotlight1.SetActive(false);
        //Activates goblin and spotlight 2
        Goblin2.SetActive(true);
        Spotlight2.SetActive(true);
        //Sets current position to 2
        currentSpot = 2;
    }
    //Switches to spot 3 for goblin and spotlight
    public void SwapTo3()
    {
        //Deactivates goblin and spotlight 2
        Goblin2.SetActive(false);
        Spotlight2.SetActive(false);
        //Activates goblin and spotlight 3
        Goblin3.SetActive(true);
        Spotlight3.SetActive(true);
        //Sets current position to 3
        currentSpot = 3;
    }

    private void Update()
    {
        //Constantly checks whether it has damaged the player
        SpotlightDamagePlayer();
    }

    void SpotlightDamagePlayer()
    {
        //Gets player position
        Player player = PlayerPosition.GetComponent<Player>();
        Vector2Int playerPosition = player.currentPosition;
        //If the player has moved, switch to the next spot
        if (playerPosition != previousPlayerPosition)
        {
            SwapPosition();
            previousPlayerPosition = playerPosition;
        }
        //Check the current spotlight damage zones
        switch (currentSpot)
        {
            //If the player is in the damage zone, they take damage from the spotlight at spot 0
            case 0:
                if (!hasPlayerTakenDamage[0] && playerPosition.x == 4 && playerPosition.y == 3)
                {
                    //Applys 10 damage to the player
                    player.TakeDamage(10);
                    //Marks that the player has taken damage from spot 0
                    hasPlayerTakenDamage[0] = true;
                }
                break;
            //If the player is in the damage zone, they take damage from the spotlight at spot 1
            case 1:
                if (!hasPlayerTakenDamage[1] && playerPosition.x == 3 && playerPosition.y == 8)
                {
                    //Applys 10 damage to the player
                    player.TakeDamage(10);
                    //Marks that the player has taken damage from spot 1
                    hasPlayerTakenDamage[1] = true;
                }
                break;
            //If the player is in the damage zone, they take damage from the spotlight at spot 2
            case 2:
                if (!hasPlayerTakenDamage[2] && playerPosition.x == 5 && playerPosition.y == 6)
                {
                    //Applys 10 damage to the player
                    player.TakeDamage(10);
                    //Marks that the player has taken damage from spot 2
                    hasPlayerTakenDamage[2] = true;
                }
                break;
            //If the player is in the damage zone, they take damage from the spotlight at spot 3
            case 3:
                if (!hasPlayerTakenDamage[3] && playerPosition.x == 6 && playerPosition.y == 4)
                {
                    //Applys 10 damage to the player
                    player.TakeDamage(10);
                    //Marks that the player has taken damage from spot 3
                    hasPlayerTakenDamage[3] = true;
                }
                break;
        }
    }
}