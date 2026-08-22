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

    public bool isAttacking = false;
    void Start()
    {
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
                GridManager.Instance.MoveEntity(entity, bestCoord);
                GridManager.Instance.RotateEntityToTarget(entity, PlayerEntity.coords);
            }
            else
            {
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

        GameObject bullet = Instantiate(projectile, spawnPoint.transform.position, spawnPoint.transform.rotation);
        bullet.transform.SetParent(GridManager.Instance.transform, true);

        Destroy(bullet, 2f);
        
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("IsAttacking", false);

        isAttacking = false;
    }
}