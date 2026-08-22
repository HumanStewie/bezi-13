using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    private Action<Vector2Int> onMoveAbilities;
    private Action onTickAbilities;
    private Action onGenericAbilities;
    private Action onUpdateAbilities;

    public bool canShoot = false;
    public bool canShoot2 = false;

    private int secondsActive = 0;
    private Entity entity;

    public GameObject legoPrefab;
    public GameObject shockWavePrefab;
    public int AttackCounter = 0;
    public bool shockWaveable;

    public bool CanReset = false;
    public bool CanReset2 = false;

    public bool CanBlock = false;
    public bool CanBlock2 = false;

    public bool Possess1 = false;
    public bool Possess2 = false;

    public bool WeightLessNess = false;

    public bool ProfessionalHater = false;

    public bool foldUnderPressure = false;

    public bool Omniboardtent = false;

    public GameObject spinningBall;

    void Start()
    {
        entity = GetComponent<Entity>();
        StartCoroutine(TimeTicker());
    }

    private void Update()
    {
        onUpdateAbilities?.Invoke();
    }

    IEnumerator TimeTicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            secondsActive++;
            onTickAbilities?.Invoke();
        }
    }

    public void OnPlayerMoved(Vector2Int oldPos) { onMoveAbilities?.Invoke(oldPos); }
    public void OnGenericAction() { onGenericAbilities?.Invoke(); }

    public void AddNewUpgrade(UpgradeData newUpgrade)
    {
        upgrades.Add(newUpgrade);

        switch (newUpgrade.IDName)
        {
            case "DotShooter": canShoot = true; break;
            case "DotShooter2": canShoot2 = true; break;

            case "FriendsInNeed": Possess1 = true; break;
            case "FriendsInNeed2": Possess1 = false; Possess2 = true; break;

            case "HatredForHand": GetComponent<Player>().damage = 8; break;
            case "HatredForHand2": GetComponent<Player>().damage = 10; break;

            case "LegoWalk": onMoveAbilities += LegoWalk; break;
            case "LegoWalk2": onMoveAbilities -= LegoWalk; onMoveAbilities += LegoWalk2; break;

            case "RageQuit": onGenericAbilities += RageQuit; break;
            case "RageQuit2": onGenericAbilities -= RageQuit; onGenericAbilities += RageQuit2; break;

            case "SpinningBall": SpinningBall(); break;
            case "SpinningBall2": SpinningBall2(); break;

            case "BlockThrower": CanBlock = true; break;
            case "BlockThrower2": CanBlock2 = true; break;

            case "GoldenWind": onTickAbilities += GoldenWind1; break;
            case "GoldenWind2": onTickAbilities -= GoldenWind1; onTickAbilities += GoldenWind2; break;

            case "PillarMan": onUpdateAbilities += PillarMan1; break;
            case "PillarMan2": onUpdateAbilities -= PillarMan1; onUpdateAbilities += PillarMan2; break;

            case "TheGreatReset": CanReset = true; break;
            case "TheGreatReset2": CanReset = false; CanReset2 = true; break;

            case "FoldUnderPressure": foldUnderPressure = true; break;
            case "OmniBoardtent": Omniboardtent = true; break;
            case "ProfessionalHater": onGenericAbilities += Professional; ProfessionalHater = true; onGenericAbilities -= RageQuit; onGenericAbilities -= RageQuit2; break;
            case "WeightLessNess": WeightLessNess = true; break;
        }
    }

    private void LegoWalk(Vector2Int oldPos)
    {
        GameObject lego = Instantiate(legoPrefab, GridManager.Instance.CoordToWorldPos(oldPos), Quaternion.identity, GridManager.Instance.transform);
        lego.GetComponent<Lego>().Damage = 3;
        lego.GetComponent<Lego>().selfDestructTime = 3;
    }

    private void LegoWalk2(Vector2Int oldPos)
    {
        GameObject lego = Instantiate(legoPrefab, GridManager.Instance.CoordToWorldPos(oldPos), Quaternion.identity, GridManager.Instance.transform);
        lego.GetComponent<Lego>().Damage = 5;
        lego.GetComponent<Lego>().selfDestructTime = 7;
    }

    private void GoldenWind1()
    {
        if (secondsActive % 10 == 0)
        {
            Vector2Int chosenTile = GridManager.Instance.SelectRandomPossible();
            GridManager.Instance.ChangeTileColor(chosenTile, Color.yellow);

            if (entity.coords == chosenTile)
            {
                StartCoroutine(FreezeGridRoutine(5f, chosenTile));
            }
        }
    }

    private IEnumerator FreezeGridRoutine(float duration, Vector2Int tile)
    {
        GridManager.Instance.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        yield return new WaitForSeconds(duration);

        GridManager.Instance.GetComponent<Rigidbody>().freezeRotation = false;
        GridManager.Instance.ChangeTileColor(tile, Color.white);
    }

    private void GoldenWind2()
    {
        if (secondsActive % 7 == 0)
        {
            Vector2Int chosenTile = GridManager.Instance.SelectRandomPossible();
            GridManager.Instance.ChangeTileColor(chosenTile, Color.yellow);

            if (entity.coords == chosenTile)
            {
                StartCoroutine(FreezeGridRoutine(4f, chosenTile));
            }
        }
    }
    private void RageQuit()
    {
        if (AttackCounter >= 4)
        {
            AttackCounter = 0;
            Vector3 spawnPos = transform.position + transform.forward * 1f;
            GameObject shockWave = Instantiate(shockWavePrefab, spawnPos, transform.rotation);
        }
    }

    private void RageQuit2()
    {
        if (AttackCounter >= 3)
        {
            AttackCounter = 0;
            Vector3 spawnPos = transform.position + transform.forward * 1f;
            GameObject shockWave = Instantiate(shockWavePrefab, spawnPos, transform.rotation);
            shockWave.GetComponent<Shockwave>().damage = 8;
            shockWave.GetComponent<Shockwave>().lifetime = 6;
        }
    }

    private void SpinningBall()
    {
        Instantiate(spinningBall);
    }
    private void SpinningBall2()
    {
        Instantiate(spinningBall);
        Instantiate(spinningBall);
    }
    private void PillarMan1() { }
    private void PillarMan2() { }
    private void Professional()
    {
        if (AttackCounter >= 3)
        {
            AttackCounter = 0;
            Vector3 spawnPos = transform.position + transform.forward * 1f;
            GameObject shockWave = Instantiate(shockWavePrefab, spawnPos, transform.rotation);
            shockWave.GetComponent<Shockwave>().damage = 10;
            shockWave.GetComponent<Shockwave>().lifetime = 6;
        }
    }
}