using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public List<GameObject> legoPrefab;
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

            case "PillarMan": PillarMan1(); break;
            case "PillarMan2": PillarMan2(); break;

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
        var rand = UnityEngine.Random.Range(0, 3);

        Node tileNode = GridManager.Instance.Grid.GetValueOrDefault(oldPos);
        if (tileNode == null) return;

        GameObject lego = Instantiate(legoPrefab[rand]);
        lego.transform.SetParent(GridManager.Instance.transform);

        Vector3 boardUp = GridManager.Instance.transform.up;
        Vector3 flatDirection = Vector3.ProjectOnPlane(transform.forward, boardUp);
        if (flatDirection != Vector3.zero)
        {
            lego.transform.rotation = Quaternion.LookRotation(flatDirection, boardUp);
        }

        lego.transform.position = tileNode.transform.position;

        lego.transform.localPosition = new Vector3(lego.transform.localPosition.x, 1.2f, lego.transform.localPosition.z);
        lego.GetComponent<Lego>().Damage = 3;
        lego.GetComponent<Lego>().selfDestructTime = 3;
    }

    private void LegoWalk2(Vector2Int oldPos)
    {
        var rand = UnityEngine.Random.Range(0, 3);

        Node tileNode = GridManager.Instance.Grid.GetValueOrDefault(oldPos);
        if (tileNode == null) return;

        GameObject lego = Instantiate(legoPrefab[rand]);
        lego.transform.SetParent(GridManager.Instance.transform);

        Vector3 boardUp = GridManager.Instance.transform.up;
        Vector3 flatDirection = Vector3.ProjectOnPlane(transform.forward, boardUp);
        if (flatDirection != Vector3.zero)
        {
            lego.transform.rotation = Quaternion.LookRotation(flatDirection, boardUp);
        }

        lego.transform.position = tileNode.transform.position;

        lego.transform.localPosition = new Vector3(lego.transform.localPosition.x, 1.2f, lego.transform.localPosition.z);
        lego.GetComponent<Lego>().Damage = 5;
        lego.GetComponent<Lego>().selfDestructTime = 7;
    }

    private void GoldenWind1()
    {
        if (secondsActive > 0 && secondsActive % 10 == 0)
        {
            StartCoroutine(GoldenWindMechanic(5f));
        }
    }

    private void GoldenWind2()
    {
        if (secondsActive > 0 && secondsActive % 7 == 0)
        {
            StartCoroutine(GoldenWindMechanic(4f));
        }
    }

    private IEnumerator GoldenWindMechanic(float freezeDuration)
    {
        Vector2Int chosenTile = GridManager.Instance.SelectRandomPossible();
        GridManager.Instance.ChangeTileColor(chosenTile, Color.goldenRod, Color.yellow);
        GridManager.Instance.ChangeIconColor(chosenTile, Color.yellow);

        TippingLogic tipping = GridManager.Instance.GetComponent<TippingLogic>();

        float timer = 0;
        while (timer < freezeDuration)
        {
            timer += Time.deltaTime;
            if (GameManager.instance.playerEntity.coords == chosenTile)
            {
                if (tipping != null) tipping.SetFreeze(true);
            }
            else
            {
                if (tipping != null) tipping.SetFreeze(false);
            }
            yield return null;
        }
        GridManager.Instance.ChangeTileColor(chosenTile, new Color(0, 0, 0.643f, 0), new Color(0, 0, 0.35f, 0));
        GridManager.Instance.ChangeIconColor(chosenTile, Color.white);

    }
    private void RageQuit()
    {
        if (AttackCounter >= 4)
        {
            AttackCounter = 0;
            Vector3 boardUp = GridManager.Instance.transform.up;

            Vector3 spawnPos = transform.position + (transform.forward * 1f) + (boardUp * 0.5f);

            GameObject shockWave = Instantiate(shockWavePrefab, spawnPos, transform.rotation, GridManager.Instance.transform);
        }
    }

    private void RageQuit2()
    {
        if (AttackCounter >= 3)
        {
            AttackCounter = 0;
            Vector3 boardUp = GridManager.Instance.transform.up;
            Vector3 spawnPos = transform.position + (transform.forward * 1f) + (boardUp * 0.5f);

            GameObject shockWave = Instantiate(shockWavePrefab, spawnPos, transform.rotation, GridManager.Instance.transform);
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
        StartCoroutine(SpawnBalls());
    }

    private IEnumerator SpawnBalls()
    {
        Instantiate(spinningBall);
        yield return new WaitForSeconds(0.5f);
        Instantiate(spinningBall);
    }
    private void PillarMan1()
    {
        TippingLogic tipping = GridManager.Instance.GetComponent<TippingLogic>();

        if (tipping != null)
        {
            tipping.ApplyPillarManBuff(1.5f, 0.8f);
        }
    }

    private void PillarMan2()
    {
        TippingLogic tipping = GridManager.Instance.GetComponent<TippingLogic>();

        if (tipping != null)
        {
            tipping.ApplyPillarManBuff(2f, 0.8f);
        }
    }
    private void Professional()
    {
        if (AttackCounter >= 3)
        {
            AttackCounter = 0;
            Vector3 boardUp = GridManager.Instance.transform.up;
            Vector3 spawnPos = transform.position + (transform.forward * 1f) + (boardUp * 0.5f);

            GameObject shockWave = Instantiate(shockWavePrefab, spawnPos, transform.rotation, GridManager.Instance.transform);
            shockWave.GetComponent<Shockwave>().damage = 10;
            shockWave.GetComponent<Shockwave>().lifetime = 6;
        }
    }
}