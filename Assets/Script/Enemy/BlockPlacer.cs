using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    bool isAttacking = false;
    Entity entity;

    public GameObject blockPrefab;
    private Vector2Int currentTargetTile;
    private bool hasTarget = false;
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
            yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);
        }
    }

    void FollowLogic()
    {
        if (!hasTarget) {
            List<Vector2Int> tiles = GridManager.Instance.GetTilesInRange(entity.coords, 6);

            var rand = UnityEngine.Random.Range(0, tiles.Count);

            currentTargetTile = tiles[rand];
            hasTarget = true;
        }

            var bestTile = entity.coords;

        var distance = 67;

        foreach(var tile in GridManager.Instance.GetTilesInRange(entity.coords, 1)) {
            if (GridManager.Instance.GetDistance(tile, currentTargetTile) < distance || GridManager.Instance.GetEntityAtPosition(tile) == null) {
                bestTile = tile;
                distance = GridManager.Instance.GetDistance(tile,currentTargetTile);
            }
        }

        Vector3 lookAtPosition = GridManager.Instance.CoordToWorldPos(bestTile);
        lookAtPosition.y = transform.position.y + 1;
        transform.LookAt(lookAtPosition);
        GridManager.Instance.MoveEntity(entity, bestTile);

        if (entity.coords == currentTargetTile) {
            hasTarget = false;
            StartCoroutine(Attacking());
        }
    }
    IEnumerator Attacking()
    {
        isAttacking = true;
        List<Vector2Int> tiles = GridManager.Instance.GetTilesInRange(entity.coords, 2);

        var rand = UnityEngine.Random.Range(0, tiles.Count);

        var randTile = tiles[rand];

        yield return new WaitForSeconds(2f);

        var block = Instantiate(blockPrefab, GridManager.Instance.CoordToWorldPos(randTile), Quaternion.identity, GridManager.Instance.transform);
        transform.LookAt(block.transform);
        isAttacking = false;
    }
}
