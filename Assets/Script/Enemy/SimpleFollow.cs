using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Follow());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator Follow()
    {
        FollowLogic();

        yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);
        StartCoroutine(Follow());
    }


    void FollowLogic()
    {
        Entity PlayerEntity = GameManager.instance.playerEntity;
        Vector2Int playerPos = PlayerEntity.coords;
        Vector2Int bestCoord = this.GetComponent<Entity>().coords;
        float distance = 50;


        if (PlayerEntity != null) {
            List<Vector2Int> possibleTile = GridManager.Instance.GetTilesInRange(this.GetComponent<Entity>().coords, 1);
            foreach (var tile in possibleTile)
            {
                if (GridManager.Instance.GetDistance(playerPos, tile) < distance)
                {
                    bestCoord = tile;
                }
            }
            GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), bestCoord);
            if (GridManager.Instance.GetDistance(playerPos, this.GetComponent<Entity>().coords) <= Mathf.Sqrt(2))
            {
                StartCoroutine(Attacking());
            }
        }
    }

    IEnumerator Attacking()
    {
        Vector2Int targetTile = GameManager.instance.playerEntity.coords;
        List<Vector2Int> dangerZone = new List<Vector2Int> { targetTile };

        TargetingController.instance.ShowAttackWarning(dangerZone, 0.5f);
        yield return new WaitForSeconds(0.5f);

        float activeDuration = 0.6f;
        float timer = 0f;
        bool hasDealtDamage = false;

        while (timer < activeDuration)
        {
            if (!hasDealtDamage && GameManager.instance.playerEntity.coords == targetTile)
            {
                GameManager.instance.playerEntity.TakeDamage(5);
                hasDealtDamage = true;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
