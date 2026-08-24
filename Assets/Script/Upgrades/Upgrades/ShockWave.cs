using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public int damage = 5;
    public float speed = 10f;
    public float lifetime = 3f;

    private HashSet<Entity> hitEntities = new HashSet<Entity>();

    void Start()
    {
        Destroy(gameObject, lifetime);
        MusicManager.Instance.PlayAbilitiesSound(transform.position, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity hitEntity = other.GetComponent<Entity>();

        if (hitEntity != null && hitEntity.entityName != "Player" && !hitEntities.Contains(hitEntity))
        {
            hitEntity.TakeDamage(damage);

            hitEntities.Add(hitEntity);
        }
    }
}