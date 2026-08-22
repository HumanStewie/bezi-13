using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float hitRadius = 0.8f;
    public int damage = 5;

    private void Update()
    {
        transform.Translate( speed * Time.deltaTime * Vector3.forward, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.playerEntity.TakeDamage(damage);
        Destroy(gameObject);
    }
}