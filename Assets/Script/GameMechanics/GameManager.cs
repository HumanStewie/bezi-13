using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float fixedSecondRate;

    public Entity playerEntity;

    public int waveNum = 1;


    public GameObject NormalHand;
    public GameObject ShootingHand;
    public GameObject Block;
    public GameObject Placer;
    public GameObject Exploder;
    public GameObject LaserShooter;
    public GameObject Tanker;
    public GameObject TheFeet;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        playerEntity = GameObject.FindWithTag("Player").GetComponent<Entity>();
    }

    void StartWave()
    {
        switch(waveNum)
        {
            case 1:
                Wave1(); break;
            case 2:
                Wave2(); break;
            case 3:
                Wave3(); break;
            case 4:
                Wave4(); break;
            case 5:
                Wave5(); break;
        }
    }



    void InstatiateEnemy(GameObject enemy)
    {
        var choosenCoord = GridManager.Instance.SelectRandomPossible();
        Instantiate(enemy, GridManager.Instance.CoordToWorldPos(choosenCoord), Quaternion.identity);
    }

    void Wave1()
    {
        float valueCost = 5.5f;
        int rand = UnityEngine.Random.Range(0, 3);

        while (valueCost > 0) {
            if (rand == 0 || rand == 1)
            {
                Instantiate(NormalHand);
                valueCost -= 1f;
            }
            else
            {
                Instantiate(ShootingHand);
                valueCost -= 1.5f;
            }
        }
    }

    void Wave2()
    {
        float valueCost = 6f;
        Instantiate(Exploder);
        int rand = UnityEngine.Random.Range(0, 4);
        while (valueCost > 0)
        {
            if (rand == 0 || rand == 1)
            {
                Instantiate(NormalHand);
                valueCost -= 1f;
            }
            else if (rand == 2)
            {
                Instantiate(ShootingHand);
                valueCost -= 1.5f;
            }
            else
            {
                Instantiate(Exploder);
                valueCost -= 1.5f;
            }
        }
    }
    void Wave3()
    {
        float valueCost = 8f;
        InstatiateEnemy(Placer);
        int rand = UnityEngine.Random.Range(0, 5);
        while (valueCost > 0)
        {
            if (rand == 0 || rand == 1)
            {
                Instantiate(NormalHand);
                valueCost -= 1f;
            }
            else if (rand == 2)
            {
                Instantiate(ShootingHand);
                valueCost -= 1.5f;
            }
            else if (rand == 3)
            {
                Instantiate(Exploder);
                valueCost -= 1.5f;
            }
            else if (rand == 4)
            {
                Instantiate(Placer);
                valueCost -= 2f;
            }
        }
    }

    void Wave4()
    {
        float valueCost = 11f;
        InstatiateEnemy(Placer);
        int rand = UnityEngine.Random.Range(0, 6);
        while (valueCost > 0)
        {
            if (rand == 0)
            {
                Instantiate(NormalHand);
                valueCost -= 1f;
            }
            else if (rand == 1 || rand == 2)
            {
                Instantiate(ShootingHand);
                valueCost -= 1.5f;
            }
            else if (rand == 3)
            {
                Instantiate(Exploder);
                valueCost -= 1.5f;
            }
            else if (rand == 5 ||  rand == 4)
            {
                Instantiate(Placer);
                valueCost -= 2f;
            }
        }
    }

    void Wave5()
    {
        float valueCost = 11f;
        InstatiateEnemy(Placer);
        int rand = UnityEngine.Random.Range(0, 6);
        while (valueCost > 0)
        {
            if (rand == 0)
            {
                Instantiate(NormalHand);
                valueCost -= 1f;
            }
            else if (rand == 1 || rand == 2)
            {
                Instantiate(ShootingHand);
                valueCost -= 1.5f;
            }
            else if (rand == 3)
            {
                Instantiate(Exploder);
                valueCost -= 1.5f;
            }
            else if (rand == 5 || rand == 4)
            {
                Instantiate(Placer);
                valueCost -= 2f;
            }
        }
    }
}
