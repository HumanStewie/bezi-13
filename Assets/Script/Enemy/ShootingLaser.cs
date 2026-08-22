using System;
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
    public float laserDuration = 0.2f; 

    private bool isAttacking = false;

    void Start()
    {
        entity = GetComponent<Entity>();
        GridManager.Instance.RegisterEntity(entity);

        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = false;
        laserLine.useWorldSpace = true; 
        animator.SetBool("IsAttacking", isAttacking);
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
        if (isAttacking)
        {
            laserLine.SetPosition(0, lazerSpawnPoint.position);
        }
    }

    // TODO: Move towards nearest player axis instead, so that the enemy doesn't awkwardly move close to us
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
        this.transform.LookAt(new Vector3(targetWorldPos.x, targetWorldPos.y, targetWorldPos.z));
        if (entity.coords.x == GameManager.instance.playerEntity.coords.x)
        {
            var dangerZone = GridManager.Instance.GetNodeByX(entity.coords.x);
            TargetingController.instance.ShowAttackWarning(dangerZone, 0.5f);
        }
        else
        {
            var dangerZone = GridManager.Instance.GetNodeByY(entity.coords.y);
            TargetingController.instance.ShowAttackWarning(dangerZone, 0.5f);
        }
        yield return new WaitForSeconds(chargeTime);

        transform.LookAt(new Vector3(targetWorldPos.x, GameManager.instance.playerEntity.transform.position.y, targetWorldPos.z));
        FireLaser();
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
        animator.SetBool("IsAttacking", false);
        animator.SetBool("HasFinished", true);
        yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);
        isAttacking = false;
    }

    void FireLaser()
    {
        Entity player = GameManager.instance.playerEntity;


        if (entity.coords.x == player.coords.x || entity.coords.y == player.coords.y)
        {
            player.TakeDamage(laserDamage);
        }

        laserLine.enabled = true;

        Vector3 startPos = lazerSpawnPoint.position;
        laserLine.SetPosition(0, startPos);

        Vector3 shootDirection = this.transform.forward;
        Vector3 endPos = startPos + (shootDirection * 500f);
        laserLine.SetPosition(1, endPos);
    }
}