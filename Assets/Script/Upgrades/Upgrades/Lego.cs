using UnityEngine;

public class Lego : MonoBehaviour
{
    public float Damage;
    public float selfDestructTime = 2f;

    public Vector2Int coords;
    void Start()
    {
        coords = GridManager.Instance.WorldToCoord(transform.position);
        Destroy(gameObject, selfDestructTime);
    }

    void Update()
    {
        Entity entityOnTile = GridManager.Instance.GetEntityAtPosition(coords);

        if (entityOnTile != null && entityOnTile.entityName != "Player")
        {
            entityOnTile.TakeDamage((int)Damage);

            Destroy(gameObject);
        }
    }
}
