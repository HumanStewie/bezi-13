using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ShootingLaser : MonoBehaviour
{
    private LineRenderer laserLine;
    private Entity entity;

    [SerializeField] private Animator animator;

    [Header("Laser Settings")]
    [SerializeField] private Transform lazerSpawnPoint;
    public int laserDamage = 5;
    public float chargeTime = 0.5f;
    public float laserDuration = 2.0f;

    private bool isAttacking = false;
    public List<Vector2Int> firezone;

    void Start()
    {
        entity = GetComponent<Entity>();

        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = false;
        laserLine.useWorldSpace = true;
        animator.SetBool("IsAttacking", isAttacking);
        GridManager.Instance.MoveEntity(entity, entity.coords, 1.5f);
        StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            if (!isAttacking && GameManager.instance.playerEntity != null)
            {
                Vector2Int playerPos = GameManager.instance.playerEntity.coords;
                Vector2Int myPos = entity.coords;

                if (myPos.x == playerPos.x || myPos.y == playerPos.y)
                {
                    yield return StartCoroutine(AttackSequence(playerPos));
                }
                else
                {
                    MoveTowardsAlignment(playerPos, myPos);
                    yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void LateUpdate()
    {
        if (isAttacking && laserLine.enabled)
        {
            laserLine.SetPosition(0, lazerSpawnPoint.position);
            laserLine.SetPosition(1, lazerSpawnPoint.position + (transform.forward * 500f));
        }
    }

    private void MoveTowardsAlignment(Vector2Int playerPos, Vector2Int myPos)
    {
        List<Vector2Int> possibleTiles = GridManager.Instance.GetTilesInRange(myPos, 1);

        Vector2Int bestCoord = myPos;
        int minDistance = int.MaxValue;
        foreach (var tile in possibleTiles)
        {
            int dist = GridManager.Instance.GetDistance(playerPos, tile);
            if (dist < minDistance)
            {
                minDistance = dist;
                bestCoord = tile;
            }
        }
        GridManager.Instance.RotateEntityToTarget(entity, bestCoord);
        GridManager.Instance.MoveEntity(entity, bestCoord, 1.5f);
    }

    private IEnumerator AttackSequence(Vector2Int lockedTargetPos)
    {
        isAttacking = true;
        animator.SetBool("HasFinished", false);
        animator.SetBool("IsAttacking", true);

        Vector3 targetWorldPos = GameManager.instance.playerEntity.transform.position;
        GridManager.Instance.RotateEntityToTargetWithY(this.transform, targetWorldPos);
        List<Vector2Int> dangerZone = new List<Vector2Int>();

        if (entity.coords.x == GameManager.instance.playerEntity.coords.x)
        {
            dangerZone = new List<Vector2Int>(GridManager.Instance.GetNodeByX(entity.coords.x));
            if (entity.coords.y <= GameManager.instance.playerEntity.coords.y)
                dangerZone.RemoveAll(zone => zone.y < entity.coords.y); 
            else
                dangerZone.RemoveAll(zone => zone.y > entity.coords.y);
        }
        else
        {
            dangerZone = new List<Vector2Int>(GridManager.Instance.GetNodeByY(entity.coords.y));
            if (entity.coords.x <= GameManager.instance.playerEntity.coords.x)
                dangerZone.RemoveAll(zone => zone.x < entity.coords.x);
            else
                dangerZone.RemoveAll(zone => zone.x > entity.coords.x);
        }

        firezone = dangerZone;
        MusicManager.Instance.PlayHandLazerSound(transform.position);
        TargetingController.instance.ShowAttackWarning(entity, dangerZone, chargeTime);
        yield return new WaitForSeconds(chargeTime);

        laserLine.enabled = true;
        MusicManager.Instance.PlayHandLazerSoundDuring(transform.position);
        float attackTimer = 0f;
        float damageTickTimer = 0f;

        while (attackTimer < laserDuration)
        {
            attackTimer += Time.deltaTime;
            damageTickTimer += Time.deltaTime;

            if (damageTickTimer >= 0.1f)
            {
                Entity player = GameManager.instance.playerEntity;
                if (firezone.Contains(player.coords))
                {
                    player.TakeDamage(laserDamage);
                }
                damageTickTimer = 0f;
            }
            yield return null;
        }

        laserLine.enabled = false;
        animator.SetBool("IsAttacking", false);
        animator.SetBool("HasFinished", true);

        yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);
        isAttacking = false;
    }
}