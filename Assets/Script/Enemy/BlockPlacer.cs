using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    private bool isAttacking = false;
    private Entity entity;

    [SerializeField] private Animator animator;
    [SerializeField] private float timeToPlaceBlock = 1f;
    public GameObject blockPrefab;
    private Vector2Int currentTargetTile;
    private bool hasTarget = false;
    void Start()
    {
        entity = GetComponent<Entity>();
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
        if (!hasTarget)
        {
            List<Vector2Int> tiles = GridManager.Instance.GetTilesInRange(entity.coords, 6);

            var rand = Random.Range(0, tiles.Count);

            currentTargetTile = tiles[rand];
            hasTarget = true;
        }

        var bestTile = entity.coords;
        var distance = 67;

        foreach (var tile in GridManager.Instance.GetTilesInRange(entity.coords, 1))
        {
            if (tile == currentTargetTile)
            {
                bestTile = tile;
                break;
            }
            if (GridManager.Instance.GetDistance(tile, currentTargetTile) < distance && GridManager.Instance.GetEntityAtPosition(tile) == null)
            {
                bestTile = tile;
                distance = GridManager.Instance.GetDistance(tile, currentTargetTile);
            }
        }

        Vector3 lookAtPosition = GridManager.Instance.CoordToWorldPos(bestTile);
        lookAtPosition.y = transform.position.y;
        transform.LookAt(lookAtPosition);
        GridManager.Instance.MoveEntity(entity, bestTile, 1.5f);

        if (entity.coords == currentTargetTile)
        {
            StartCoroutine(Attacking());
        }
    }
    IEnumerator Attacking()
    {
        isAttacking = true;
        List<Vector2Int> tiles = GridManager.Instance.GetTilesInRange(entity.coords, 2);

        var rand = Random.Range(0, tiles.Count);

        var randTile = tiles[rand];
        /*var randTileWorldPos = GridManager.Instance.GetNode(randTile).transform.position;
        randTileWorldPos.y = transform.position.y + 1;*/
        var randTileWorldPos = GridManager.Instance.CoordToWorldPos(randTile);
        transform.LookAt(randTileWorldPos);

        animator.SetBool("IsAttacking", true);
        yield return new WaitForSeconds(timeToPlaceBlock);

        var block = Instantiate(blockPrefab, GridManager.Instance.CoordToWorldPos(randTile), Quaternion.identity, GridManager.Instance.transform);
        transform.LookAt(block.transform);

        animator.SetBool("IsAttacking", false);
        isAttacking = false;
        hasTarget = false;
    }
}