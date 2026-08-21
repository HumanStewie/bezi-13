using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : MonoBehaviour
{
    bool isAttacking = false;
    Entity entity;
    void Start()
    {
        entity = GetComponent<Entity>();
        StartCoroutine(BehaviourLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BehaviourLoop()
    {
        while (true)
        {
            if (!isAttacking)
            {
                FollowLogic();
            }
            yield return new WaitForSeconds(GameManager.instance.fixedSecondRate/2);
        }
    }

    void FollowLogic()
    {
        var playerEntity = GameManager.instance.playerEntity;
        var playerCoord = playerEntity.coords;


        Vector2Int bestCoord = entity.coords;

        float distance = 60;

        foreach(var tile in GridManager.Instance.GetTilesInRange(entity.coords,1))
        {
            if (GridManager.Instance.GetDistance(tile, playerCoord) < distance) {
                bestCoord = tile;
                distance = GridManager.Instance.GetDistance(tile, bestCoord);
            }
        }
        GridManager.Instance.MoveEntity(entity, bestCoord);
        if (GridManager.Instance.GetDistance(entity.coords, playerCoord) <= 2)
        {
            StartCoroutine(Attacking());
        }
    }
    IEnumerator Attacking()
    {
        isAttacking = true;
        List<Vector2Int> targetTiles = GridManager.Instance.GetTilesInRange(entity.coords, 3);
        targetTiles.Add(entity.coords);
        TargetingController.instance.ShowAttackWarning(targetTiles, 0.5f);

        Vector3 originalScale = this.transform.localScale;

        float timer = 0;

        float pulseSpeed = 25f;
        float pulseAmount = 0.3f;

        while (timer <= 0.4f)
        {
            float scaleMultiplier = 1f + (Mathf.Sin(timer * pulseSpeed) * pulseAmount);
            transform.localScale = originalScale * scaleMultiplier;

            timer += Time.deltaTime;

            yield return null;
        }

        timer = 0f;
        Vector3 currentScale = transform.localScale;
        Vector3 explodingScale = originalScale * 2f;
        while (timer < 0.5f)
        {
            float percentComplete = timer / 0.1f;
            transform.localScale = Vector3.Lerp(currentScale, explodingScale, percentComplete);

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var tile in targetTiles) {
            var Newentity = GridManager.Instance.GetEntityAtPosition(tile);
            if (Newentity)
            {
                Newentity.TakeDamage(10f);
            }
        }
        Destroy(this.gameObject);
    }
}
