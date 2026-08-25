using UnityEngine;

public class Lego : MonoBehaviour
{
    public float Damage;
    public float selfDestructTime = 5f;

    public Vector2Int coords;
    void Start()
    {
        coords = GridManager.Instance.WorldToCoord(transform.position);
        if (!FindAnyObjectByType<PlayerUpgrades>().foldUnderPressure)
        {
            Destroy(gameObject, selfDestructTime);
        }
        else
        {
            Invoke("Explode", 3);
        }
    }

    void Update()
    {
        Entity entityOnTile = GridManager.Instance.GetEntityAtPosition(coords);

        if (entityOnTile != null && entityOnTile.entityName != "Player" && entityOnTile.entityName != "Block")
        {
            entityOnTile.TakeDamage((int)Damage);

            Destroy(gameObject);
        }
    }

    void Explode()
    {
        var targetTiles = GridManager.Instance.GetTilesInRangeIncludesSquareRoot(this.coords, 1);
        foreach (var tile in targetTiles)
        {
            var Newentity = GridManager.Instance.GetEntityAtPosition(tile);
            if (Newentity)
            {
                Newentity.TakeDamage(10f);
            }
        }
        CameraShake.Instance.ShakeCamera(0.2f, 0.2f);
        Destroy(gameObject,0.2f);
    }
}
