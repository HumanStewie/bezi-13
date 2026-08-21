using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float hitRadius = 0.8f;
    public int damage = 5;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        if (GameManager.instance.playerEntity != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, GameManager.instance.playerEntity.transform.position);

            if (distToPlayer <= hitRadius)
            {
                GameManager.instance.playerEntity.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}