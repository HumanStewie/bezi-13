using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    private Action<Vector2Int> onMoveAbilities;
    private Action onTickAbilities;
    private Action onAttackAbilities;

    private Action onGenericAbilities;

    public bool canShoot;
    public bool canShoot2;

    private int secondsActive = 0;
    private Entity entity;

    public GameObject legoPrefab;
    public GameObject shockWavePrefab;
    public int AttackCounter = 0;
    public bool shockWaveable;

    void Start()
    {
        entity = GetComponent<Entity>();
        StartCoroutine(TimeTicker());
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

    private void Update()
    {
        onAttackAbilities.Invoke();
    }
    public void AddNewUpgrade(UpgradeData newUpgrade)
    {
        upgrades.Add(newUpgrade);

        switch (newUpgrade.IDName)
        {

            case "DotShooter": canShoot = true; break;
            case "DotShooter2": canShoot2 = true; break;

            case "FriendsInNeed": onGenericAbilities += FriendsInNeed1; break; 
            case "FriendsInNeed2": onGenericAbilities += FriendsInNeed2; break;

            case "HatredForHand": GetComponent<Player>().damage = 8; break;
            case "HatredForHand2": GetComponent<Player>().damage = 10; break;

            case "LegoWalk": onMoveAbilities += LegoWalk; break;
            case "LegoWalk2": onMoveAbilities += LegoWalk2; break;

            case "RageQuit": onGenericAbilities += RageQuit; break;
            case "RageQuit2": onAttackAbilities -= RageQuit; onAttackAbilities += RageQuit2; break;

            case "SpinningBall": onGenericAbilities += SpinningBall; break;
            case "SpinningBall2": onGenericAbilities += SpinningBall2; break;


            case "BlockThrower": onGenericAbilities += BlockThrower; break;
            case "BlockThrower2": onGenericAbilities += BlockThrower2; break;

            case "GoldenWind": onTickAbilities += GoldenWind1; break;
            case "GoldenWind2": onTickAbilities -= GoldenWind1; onTickAbilities += GoldenWind2; break;

            case "PillarMan": onGenericAbilities += PillarMan1; break;
            case "PillarMan2":onGenericAbilities += PillarMan2; break;

            case "TheGreatReset": onGenericAbilities += TheGreatReset1; break;
            case "TheGreatReset2": onGenericAbilities += TheGreatReset2; break;

            case "FoldUnderPressure": onGenericAbilities += FoldUnderPressure; break; 
            case "OmniBoardtent": onGenericAbilities += OmniBoard; break;
            case "ProfessionalHater": onGenericAbilities += Professional; break;
            case "WeightLessNess": onGenericAbilities += WeightLess; break;
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
                float time = 5f;
                while (time >= 0)
                {
                    GridManager.Instance.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    time -= Time.deltaTime;
                }
                GridManager.Instance.GetComponent<Rigidbody>().freezeRotation = false;

                GridManager.Instance.ChangeTileColor(chosenTile, Color.white);
            }
        } 
    }

    private void GoldenWind2()
    {
        if (secondsActive % 7 == 0)
        {
            Vector2Int chosenTile = GridManager.Instance.SelectRandomPossible();
            GridManager.Instance.ChangeTileColor(chosenTile, Color.yellow);
            if (entity.coords == chosenTile)
            {
                float time = 4f;
                while (time >= 0)
                {
                    GridManager.Instance.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    time -= Time.deltaTime;
                }
                GridManager.Instance.GetComponent<Rigidbody>().freezeRotation = false;

                GridManager.Instance.ChangeTileColor(chosenTile, Color.white);
            }
        }
    }
    private void BlockThrower() { }
    private void BlockThrower2() { }
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

    private void TheGreatReset1() { }
    private void TheGreatReset2() { }

    private void SpinningBall() { }
    private void SpinningBall2() { }
    private void PillarMan1() { }
    private void PillarMan2() { }
    private void OmniBoard() { }
    private void Professional() { }
    private void WeightLess() { }

    private void FriendsInNeed1() { }
    private void FriendsInNeed2() { }
    private void FoldUnderPressure() { }
}