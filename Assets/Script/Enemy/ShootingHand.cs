using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingHand : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Entity entity;
    public GameObject projectile;
    public float bulletSpeed = 15f;
    public float maxDistance = 30f;
    public float distanceTraveled = 0f;
    public float hitRadius = 0.8f;
    public int distanceBeforeShoot = 4;

    [SerializeField] private float ChargingTime = 2;

    public bool isAttacking = false;
    void Start()
    {
        GridManager.Instance.RegisterEntity(entity);
        GridManager.Instance.MoveEntity(entity, entity.coords, 1.5f);
        StartCoroutine(BehaviourLoop());
    }
    IEnumerator BehaviourLoop()
    {
        while (true)
        {
            if (!isAttacking)
            {
                FollowLogic();
            }
            yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);
        }
    }


    void FollowLogic()
    {
        Entity PlayerEntity = GameManager.instance.playerEntity;
        Vector2Int playerPos = PlayerEntity.coords;
        Vector2Int bestCoord = entity.coords;
        float distance = 50;


        if (PlayerEntity != null)
        {
            if (GridManager.Instance.GetDistance(playerPos,entity.coords) >= distanceBeforeShoot)
            {
                List<Vector2Int> possibleTile = GridManager.Instance.GetTilesInRange(entity.coords, 1);
                foreach (var tile in possibleTile)
                {
                    if (GridManager.Instance.GetDistance(playerPos, tile) < distance && GridManager.Instance.GetEntityAtPosition(tile) == null)
                    {
                        bestCoord = tile;
                        distance = GridManager.Instance.GetDistance(playerPos, tile);
                    }
                }
                GridManager.Instance.MoveEntity(entity, bestCoord, 1.5f);
                GridManager.Instance.RotateEntityToTarget(entity, PlayerEntity.coords);
            }
            else
            {
                GridManager.Instance.MoveEntity(entity, bestCoord, 1.5f);
                StartCoroutine(Attacking());
            }
        }
    }

    IEnumerator Attacking()
    {
        animator.SetBool("IsAttacking", true);
        isAttacking = true;
        Vector3 targetPos = GameManager.instance.playerEntity.transform.position;
        this.transform.LookAt(targetPos);
        List<Vector2Int> newList = new List<Vector2Int>() { GetComponent<Entity>().coords };
        TargetingController.instance.ShowAttackWarning(newList, ChargingTime);
        yield return new WaitForSeconds(ChargingTime);
        GameObject bullet = Instantiate(projectile, spawnPoint.transform.position, spawnPoint.transform.rotation);
        MusicManager.Instance.PlayHandShootSound(transform.position);
        bullet.transform.SetParent(GridManager.Instance.transform, true);

        Destroy(bullet, 4f);
        

        yield return new WaitForSeconds(0.67f);
        animator.SetBool("IsAttacking", false);
        
        isAttacking = false;
    }
}