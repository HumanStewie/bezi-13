using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float fixedSecondRate;

    public Entity playerEntity;

    public int currentWave = 0;


    public GameObject NormalHand;
    public GameObject ShootingHand;
    public GameObject Block;
    public GameObject Placer;
    public GameObject Exploder;
    public GameObject LaserShooter;
    public GameObject Tanker;
    public GameObject TheFeet;


    [SerializeField] private bool started = false;
    [SerializeField] private bool checking = false;
    private void Awake()
    {
        instance = this;
    }

    void StartWave(int wave)
    {
        switch(wave)
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

    void CheckIfWaveDone()
    {
        if (started && !checking)
        {
            Entity[] entities = FindObjectsByType<Entity>(FindObjectsSortMode.None);
            bool enemiesStillAlive = false;

            foreach (Entity entity in entities)
            {
                if (entity.entityName != "Player")
                {
                    enemiesStillAlive = true;
                    break;
                }
            }

            if (!enemiesStillAlive)
            {
                checking = true;

                UpgradeGrantingLogic.instance.StartGrantingUpgrades();
                FindAnyObjectByType<Player>().GetComponent<Player>().enabled = false;
            }
        }
    }


    public void ProceedNextWave()
    {
        checking = false;
        FindAnyObjectByType<Player>().GetComponent<Player>().enabled = true;
        StartWave(currentWave += 1);
    }
 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !started)
        {
            StartWave(currentWave += 1);
            started = true;
        }
        CheckIfWaveDone();
    }

    void InstatiateEnemy(GameObject enemy)
    {
        var choosenCoord = GridManager.Instance.SelectRandomPossible();
        Instantiate(enemy, GridManager.Instance.CoordToWorldPos(choosenCoord), Quaternion.identity);
    }

    void Wave1()
    {
        float valueCost = 5.5f;

        while (valueCost > 0) {
            int rand = UnityEngine.Random.Range(0, 3);
            if (rand == 0 || rand == 1)
            {
                InstatiateEnemy(NormalHand);
                valueCost -= 1f;
            }
            else
            {
                InstatiateEnemy(ShootingHand);
                valueCost -= 1.5f;
            }
        }
    }

    void Wave2()
    {
        float valueCost = 6f;
        Instantiate(Exploder);
        while (valueCost > 0)
        {
            int rand = UnityEngine.Random.Range(0, 4);

            if (rand == 0 || rand == 1)
            {
                InstatiateEnemy(NormalHand);
                valueCost -= 1f;
            }
            else if (rand == 2)
            {
                InstatiateEnemy(ShootingHand);
                valueCost -= 1.5f;
            }
            else
            {
                InstatiateEnemy(Exploder);
                valueCost -= 1.5f;
            }
        }
    }
    void Wave3()
    {
        float valueCost = 8f;
        InstatiateEnemy(Placer);
        while (valueCost > 0)
        {
            int rand = UnityEngine.Random.Range(0, 4);

            if (rand == 0 || rand == 1)
            {
                InstatiateEnemy(NormalHand);
                valueCost -= 1f;
            }
            else if (rand == 2)
            {
                InstatiateEnemy(ShootingHand);
                valueCost -= 1.5f;
            }
            else if (rand == 3)
            {
                InstatiateEnemy(Exploder);
                valueCost -= 1.5f;
            }
            else if (rand == 4)
            {
                InstatiateEnemy(Placer);
                valueCost -= 2f;
            }
        }
    }

    void Wave4()
    {
        float valueCost = 11f;
        InstatiateEnemy(Placer);
        while (valueCost > 0)
        {
            int rand = UnityEngine.Random.Range(0, 4);

            if (rand == 0)
            {
                InstatiateEnemy(NormalHand);
                valueCost -= 1f;
            }
            else if (rand == 1 || rand == 2)
            {
                InstatiateEnemy(ShootingHand);
                valueCost -= 1.5f;
            }
            else if (rand == 3)
            {
                InstatiateEnemy(Exploder);
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
        while (valueCost > 0)
        {
            int rand = UnityEngine.Random.Range(0, 4);

            if (rand == 0)
            {
                InstatiateEnemy(NormalHand);
                valueCost -= 1f;
            }
            else if (rand == 1 || rand == 2)
            {
                InstatiateEnemy(ShootingHand);
                valueCost -= 1.5f;
            }
            else if (rand == 3)
            {
                InstatiateEnemy(Exploder);
                valueCost -= 1.5f;
            }
            else if (rand == 5 || rand == 4)
            {
                InstatiateEnemy(Placer);
                valueCost -= 2f;
            }
        }
    }
}
