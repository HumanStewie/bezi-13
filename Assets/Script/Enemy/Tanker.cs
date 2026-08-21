using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : MonoBehaviour
{
    private bool isAttacking = false;
    void Start()
    {
        StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            FollowLogic();
            yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);
        }
    }
    void FollowLogic()
    {
        Entity PlayerEntity = GameManager.instance.playerEntity;
        Vector2Int playerPos = PlayerEntity.coords;
        Vector2Int bestCoord = this.GetComponent<Entity>().coords;
        float distance = 50;


        if (PlayerEntity != null)
        {
            List<Vector2Int> possibleTile = GridManager.Instance.GetTilesInRange(this.GetComponent<Entity>().coords, 1);
            foreach (var tile in possibleTile)
            {
                if (GridManager.Instance.GetDistance(playerPos, tile) > distance)
                {
                    bestCoord = tile;
                    distance = GridManager.Instance.GetDistance(playerPos, tile);
                }
            }
            GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), bestCoord);
        }
    }
}
