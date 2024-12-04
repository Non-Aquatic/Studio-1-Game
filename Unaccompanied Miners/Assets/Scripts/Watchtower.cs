using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Watchtower : MonoBehaviour
{
    public GameObject PlayerPosition;

    public GameObject Goblin0;
    public GameObject Goblin1;
    public GameObject Goblin2;
    public GameObject Goblin3;

    public GameObject Spotlight0;
    public GameObject Spotlight1;
    public GameObject Spotlight2;
    public GameObject Spotlight3;

    public int currentSpot;

    public void Start()
    {
        currentSpot = 0;
        Goblin0.SetActive(true);
        Goblin1.SetActive(false);
        Goblin2.SetActive(false);
        Goblin3.SetActive(false);

        Spotlight0.SetActive(true);
        Spotlight1.SetActive(false);
        Spotlight2.SetActive(false);
        Spotlight3.SetActive(false);
    }

    public void SwapPosition()
    {
        if (currentSpot == 0)
        {
            SwapTo1();
        }
        else if (currentSpot == 1)
        {
            SwapTo2();
        }
        else if (currentSpot == 2)
        {
            SwapTo3();
        }
        else if (currentSpot == 3)
        {
            SwapTo0();
        }
    }

    public void SwapTo0()
    {
        Goblin3.SetActive(false);
        Spotlight3.SetActive(false);
        Goblin0.SetActive(true);
        Spotlight0.SetActive(true);
        currentSpot = 0;
    }

    public void SwapTo1()
    {
        Goblin0.SetActive(false);
        Spotlight0.SetActive(false);
        Goblin1.SetActive(true);
        Spotlight1.SetActive(true);
        currentSpot = 1;
    }

    public void SwapTo2()
    {
        Goblin1.SetActive(false);
        Spotlight1.SetActive(false);
        Goblin2.SetActive(true);
        Spotlight2.SetActive(true);
        currentSpot = 2;
    }

    public void SwapTo3()
    {
        Goblin2.SetActive(false);
        Spotlight2.SetActive(false);
        Goblin3.SetActive(true);
        Spotlight3.SetActive(true);
        currentSpot = 3;
    }

    private void Update()
    {
        SpotlightDamagePlayer();
    }

    void SpotlightDamagePlayer()
    {
        Player player = PlayerPosition.GetComponent<Player>();
        Vector2Int playerPosition = player.currentPosition;

        switch (currentSpot)
        {
            case 0:
                if (playerPosition.x == 1 && playerPosition.y == 3 && currentSpot == 0)
                {
                    player.TakeDamage(10);
                }
                break;
            case 1:
                if (playerPosition.x == 0 && playerPosition.y == 5 && currentSpot == 1)
                {
                    player.TakeDamage(10);
                }
                break;
            case 2:
                if (playerPosition.x == 2 && playerPosition.y == 6 && currentSpot == 2)
                {
                    player.TakeDamage(10);
                }
                break;
            case 3:
                if (playerPosition.x == 3 && playerPosition.y == 4 && currentSpot == 3)
                {
                    player.TakeDamage(10);
                }
                break;
        }
    }
}