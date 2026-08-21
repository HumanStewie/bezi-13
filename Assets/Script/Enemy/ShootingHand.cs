using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingHand : MonoBehaviour
{
    public GameObject projectile;
    public float bulletSpeed = 15f;
    public float maxDistance = 30f;
    public float distanceTraveled = 0f;
    public float hitRadius = 0.8f;


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
        Vector2Int bestCoord = this.GetComponent<Entity>().coords;
        float distance = 50;


        if (PlayerEntity != null)
        {
            if (GridManager.Instance.GetDistance(playerPos,this.GetComponent<Entity>().coords) >= 4)
            {
                List<Vector2Int> possibleTile = GridManager.Instance.GetTilesInRange(this.GetComponent<Entity>().coords, 1);
                foreach (var tile in possibleTile)
                {
                    if (GridManager.Instance.GetDistance(playerPos, tile) < distance)
                    {
                        bestCoord = tile;
                        distance = GridManager.Instance.GetDistance(playerPos, tile);
                    }
                }
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), bestCoord);
            }
            else
            {
                StartCoroutine(Attacking());
            }
        }
    }

    IEnumerator Attacking()
    {
        isAttacking = true;
        Vector3 targetPos = GameManager.instance.playerEntity.transform.position;
        this.transform.LookAt(targetPos);

        GameObject bullet = Instantiate(projectile, this.transform.position, this.transform.rotation);
        bullet.transform.SetParent(GridManager.Instance.transform, true);

        Destroy(bullet, 2f);

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }
}