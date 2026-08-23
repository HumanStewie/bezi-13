using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float fixedSecondRate;

    public Entity playerEntity;
    private Player player;
    private PlayerUpgrades playerUpgrades;

    public int currentWave = 0;


    [SerializeField] private GameObject NormalHand;
    [SerializeField] private GameObject ShootingHand;
    [SerializeField] private GameObject Block;
    [SerializeField] private GameObject Placer;
    [SerializeField] private GameObject Exploder;
    [SerializeField] private GameObject LaserShooter;
    [SerializeField] private GameObject Tanker;
    [SerializeField] private GameObject TheFeet;

    int resetCount = 0;
    [SerializeField] private KeyCode ResetKeyCode;

    [SerializeField] private bool started = false;
    [SerializeField] private bool checking = false;


    [SerializeField] private Material weightLess;

    public int enemyKillCount = 0; 
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        playerUpgrades = playerEntity.GetComponent<PlayerUpgrades>();
        player = playerEntity.GetComponent<Player>();   
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
                if (entity.entityName != "Player" && entity.entityName != "Block")
                {
                    enemiesStillAlive = true;
                    break;
                }
            }

            if (!enemiesStillAlive)
            {
                checking = true;

                UpgradeGrantingLogic.instance.StartGrantingUpgrades();
                player.enabled = false;
            }
        }
    }


    public void ProceedNextWave()
    {
        checking = false;
        player.enabled = true;
        StartWave(currentWave += 1);

        
        
        
        player.ResetAllBlock();

        PossessEnemies();
        if (playerUpgrades.CanBlock)
        {
            player.SummonBlock();
        }

        if (playerUpgrades.CanReset)
        {
            resetCount = 1;
        }
        else if (playerUpgrades.CanReset2)
        {
            resetCount = 2;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !started)
        {
            StartWave(currentWave += 1);
            started = true;
        }
        CheckIfWaveDone();

        if (Input.GetKeyDown(ResetKeyCode) && resetCount > 0)
        {
            resetCount -= 1;
            ResetBoardPhysics();
        }

        if (playerUpgrades.Omniboardtent && enemyKillCount >= 5)
        {
            enemyKillCount = 0;
            ResetBoardPhysics();
        }
    }
    private void ResetBoardPhysics()
    {
        GridManager.Instance.transform.rotation = Quaternion.identity;

        TippingLogic tipping = GridManager.Instance.GetComponent<TippingLogic>();
        if (tipping != null) tipping.ForceResetTilt();
    }

    private void PossessEnemies()
    {
        Entity[] entities = FindObjectsByType<Entity>(FindObjectsSortMode.None);
        List<Entity> pool = new List<Entity>();

        foreach (Entity e in entities)
        {
            if (e.entityName != "Player" && e.entityName != "Block")
            {
                pool.Add(e);
            }
        }

        int amountToPick = Mathf.Min(3, pool.Count);
        if (amountToPick == 0) return;

        List<Entity> chosenItems = new List<Entity>();
        for (int i = 0; i < amountToPick; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pool.Count);
            chosenItems.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        if (playerUpgrades.Possess1)
        {
            chosenItems[0].weight = 0;
            AddMaterial(chosenItems[0].GetComponentInChildren<SkinnedMeshRenderer>(), weightLess);
        }
        else if (playerUpgrades.Possess2)
        {
            for (int i = 0; i < chosenItems.Count; i++)
            {
                chosenItems[i].weight = 0;
                AddMaterial(chosenItems[i].GetComponentInChildren<SkinnedMeshRenderer>(), weightLess);
            }
        }
    }

    void AddMaterial(SkinnedMeshRenderer skinnedMeshRenderer, Material extraMat)
    {
        if (skinnedMeshRenderer == null || extraMat == null) return;

        Material[] currentMats = skinnedMeshRenderer.materials;

        Material[] newMats = new Material[currentMats.Length + 1];

        for (int i = 0; i < currentMats.Length; i++)
        {
            newMats[i] = currentMats[i];
        }

        newMats[newMats.Length - 1] = extraMat;

        skinnedMeshRenderer.materials = newMats;
    }

    void InstatiateEnemy(GameObject enemy)
    {
        var choosenCoord = GridManager.Instance.SelectRandomPossible();
        Instantiate(enemy, GridManager.Instance.CoordToWorldPos(choosenCoord), Quaternion.identity);
    }

    void Wave1()
    {
        float valueCost = 3.5f;

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
