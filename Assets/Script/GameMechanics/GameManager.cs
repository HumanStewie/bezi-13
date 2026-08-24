using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float fixedSecondRate;

    public Entity playerEntity;
    private Player player;
    private PlayerUpgrades playerUpgrades;

    public int currentWave = 0;

    [SerializeField] private TextMeshProUGUI waveText;

    [SerializeField] private GameObject NormalHand;
    [SerializeField] private GameObject ShootingHand;
    [SerializeField] private GameObject Block;
    [SerializeField] private GameObject Placer;
    [SerializeField] private GameObject Exploder;
    [SerializeField] private GameObject LaserShooter;
    [SerializeField] private GameObject Tanker;

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
            case 6:
                Wave6(); break;
            case 7:
                Wave7(); break;
            case 8:
                Wave8(); break;
            case 9:
                Wave9(); break;
            case 10:
                Wave10(); break;
            default: Wave10(); break;
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
                player.enabled = false; 

                StartCoroutine(AnimateWaveText("WAVE CLEARED", () =>
                {
                    UpgradeGrantingLogic.instance.StartGrantingUpgrades();
                }));
            }
        }
    }


    public void ProceedNextWave()
    {
        player.enabled = true;

        player.ResetAllBlock();
        PossessEnemies();

        if (playerUpgrades.CanBlock) { player.SummonBlock(); }
        if (playerUpgrades.CanReset) { resetCount = 1; }
        else if (playerUpgrades.CanReset2) { resetCount = 2; }

        currentWave++;
        StartCoroutine(AnimateWaveText("WAVE " + currentWave, () =>
        {
            StartWave(currentWave);
            checking = false;
        }));
    }

    private IEnumerator AnimateWaveText(string message, Action onAnimationComplete)
    {
        waveText.text = message;
        waveText.gameObject.SetActive(true);

        RectTransform rect = waveText.rectTransform;
        Vector2 originalPos = Vector2.zero;

        Color originalColor = waveText.color;
        originalColor.a = 1f;
        waveText.color = originalColor;

        float duration = 2f;
        float elapsed = 0f;
        float shakeIntensity = 15f; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            float currentScale = Mathf.Lerp(0.5f, 2.5f, percent);
            rect.localScale = new Vector3(currentScale, currentScale, 1f);

            rect.anchoredPosition = originalPos + new Vector2(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f)) * shakeIntensity;

            if (percent > 0.5f)
            {
                float fadePercent = (percent - 0.5f) * 2f;
                originalColor.a = Mathf.Lerp(1f, 0f, fadePercent);
                waveText.color = originalColor;
            }

            yield return null; 
        }

        waveText.gameObject.SetActive(false);
        rect.anchoredPosition = originalPos;

        onAnimationComplete?.Invoke(); 
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !started)
        {
            started = true;
            checking = true;
            currentWave++;

            StartCoroutine(AnimateWaveText("WAVE " + currentWave, () =>
            {
                StartWave(currentWave);
                checking = false; 
            }));
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
        float valueCost = 5f;
        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 70) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
        }
    }

    void Wave2()
    {
        float valueCost = 8f;
        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 60) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
        }
    }


    void Wave3()
    {
        float valueCost = 11f;
        InstatiateEnemy(Exploder); 
        valueCost -= 1.5f;

        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 50) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else if (roll < 80) { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
            else { InstatiateEnemy(Exploder); valueCost -= 1.5f; }
        }
    }

    void Wave4()
    {
        float valueCost = 14f;
        InstatiateEnemy(Placer); 
        valueCost -= 2f;

        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 40) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else if (roll < 65) { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
            else if (roll < 85) { InstatiateEnemy(Exploder); valueCost -= 1.5f; }
            else { InstatiateEnemy(Placer); valueCost -= 2f; }
        }
    }

    void Wave5()
    {
        float valueCost = 17f;
        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 35) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else if (roll < 60) { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
            else if (roll < 80) { InstatiateEnemy(Exploder); valueCost -= 1.5f; }
            else { InstatiateEnemy(Placer); valueCost -= 2f; }
        }
    }


    void Wave6()
    {
        float valueCost = 20f;
        InstatiateEnemy(LaserShooter);
        valueCost -= 2.5f;

        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 30) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else if (roll < 50) { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
            else if (roll < 70) { InstatiateEnemy(Exploder); valueCost -= 1.5f; }
            else if (roll < 85) { InstatiateEnemy(Placer); valueCost -= 2f; }
            else { InstatiateEnemy(LaserShooter); valueCost -= 2.5f; }
        }
    }

    void Wave7()
    {
        float valueCost = 24f;
        InstatiateEnemy(Tanker);
        valueCost -= 5f;

        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 25) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else if (roll < 45) { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
            else if (roll < 60) { InstatiateEnemy(Exploder); valueCost -= 1.5f; }
            else if (roll < 80) { InstatiateEnemy(Placer); valueCost -= 2f; }
            else if (roll < 95) { InstatiateEnemy(LaserShooter); valueCost -= 2.5f; }
            else { InstatiateEnemy(Tanker); valueCost -= 5f; } 
        }
    }


    void Wave8()
    {
        float valueCost = 28f;
        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 15) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else if (roll < 30) { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
            else if (roll < 50) { InstatiateEnemy(Exploder); valueCost -= 1.5f; }
            else if (roll < 70) { InstatiateEnemy(Placer); valueCost -= 2f; }
            else if (roll < 90) { InstatiateEnemy(LaserShooter); valueCost -= 2.5f; }
            else { InstatiateEnemy(Tanker); valueCost -= 5f; }
        }
    }

    void Wave9()
    {
        float valueCost = 32f;
        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 15) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else if (roll < 30) { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
            else if (roll < 50) { InstatiateEnemy(Exploder); valueCost -= 1.5f; }
            else if (roll < 65) { InstatiateEnemy(Placer); valueCost -= 2f; }
            else if (roll < 85) { InstatiateEnemy(LaserShooter); valueCost -= 2.5f; }
            else { InstatiateEnemy(Tanker); valueCost -= 5f; }
        }
    }

    void Wave10()
    {
        float valueCost = 38f;

        InstatiateEnemy(Tanker);
        InstatiateEnemy(Tanker);
        valueCost -= 10f;

        while (valueCost > 0)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll < 10) { InstatiateEnemy(NormalHand); valueCost -= 1f; }
            else if (roll < 20) { InstatiateEnemy(ShootingHand); valueCost -= 1.5f; }
            else if (roll < 40) { InstatiateEnemy(Exploder); valueCost -= 1.5f; }
            else if (roll < 60) { InstatiateEnemy(Placer); valueCost -= 2f; }
            else if (roll < 80) { InstatiateEnemy(LaserShooter); valueCost -= 2.5f; }
            else { InstatiateEnemy(Tanker); valueCost -= 5f; }
        }
    }
}
