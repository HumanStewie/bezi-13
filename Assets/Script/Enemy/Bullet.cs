using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float hitRadius = 0.8f;
    public int damage = 5;
    public bool player = false;


    public LayerMask layermask;

    private void Update()
    {
        transform.Translate( speed * Time.deltaTime * Vector3.forward, Space.Self);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity hitEntity = other.GetComponent<Entity>();

        if (hitEntity != null)
        {
            if (!player)
            {
                if (hitEntity.entityName == "Player")
                {
                    GameManager.instance.playerEntity.TakeDamage(damage);
                    Destroy(gameObject);
                }
            }
            if (player) {
                if (other.TryGetComponent(out Entity entity))
                {
                    if (entity.name != "Player")
                    {
                        entity.TakeDamage(damage);
                    }
                }
            }
        }
    }
}