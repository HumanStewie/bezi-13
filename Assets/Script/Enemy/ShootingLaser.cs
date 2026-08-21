using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ShootingLaser : MonoBehaviour
{
    private LineRenderer laserLine;
    private Entity myEntity;

    [Header("Laser Settings")]
    public int laserDamage = 5;
    public float chargeTime = 0.5f;   
    public float laserDuration = 0.2f; 

    private bool isAttacking = false;

    void Start()
    {
        myEntity = GetComponent<Entity>();

        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = false;
        laserLine.useWorldSpace = true; 

        StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            if (!isAttacking && GameManager.instance.playerEntity != null)
            {
                Vector2Int playerPos = GameManager.instance.playerEntity.coords;
                Vector2Int myPos = myEntity.coords;

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

    void MoveTowardsAlignment(Vector2Int playerPos, Vector2Int myPos)
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

        GridManager.Instance.MoveEntity(myEntity, bestCoord);
    }

    IEnumerator AttackSequence(Vector2Int lockedTargetPos)
    {
        isAttacking = true;

        Vector3 targetWorldPos = GameManager.instance.playerEntity.transform.position;
        this.transform.LookAt(new Vector3(targetWorldPos.x, this.transform.position.y, targetWorldPos.z));

        yield return new WaitForSeconds(chargeTime);

        FireLaser();

        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;

        yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);

        isAttacking = false;
    }

    void FireLaser()
    {
        Entity player = GameManager.instance.playerEntity;


        if (myEntity.coords.x == player.coords.x || myEntity.coords.y == player.coords.y)
        {
            player.TakeDamage(laserDamage);
            Debug.Log("ZAP! Laser hit the player!");
        }

        laserLine.enabled = true;

        Vector3 startPos = this.transform.position + new Vector3(0, 0.5f, 0);
        laserLine.SetPosition(0, startPos);

        Vector3 shootDirection = this.transform.forward;
        Vector3 endPos = startPos + (shootDirection * 50f);
        laserLine.SetPosition(1, endPos);
    }
}