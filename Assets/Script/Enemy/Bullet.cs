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

    private void Update()
    {
        transform.Translate( speed * Time.deltaTime * Vector3.forward, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!player)
        {
            if (other.GetComponent<Entity>() != null) { if (other.GetComponent<Entity>().entityName == "Player") GameManager.instance.playerEntity.TakeDamage(damage); } 

        }
        else
        {
            if (other.GetComponent<Entity>() != null) { other.GetComponent<Entity>().TakeDamage(damage); }
        }
        Destroy(gameObject);
    }
}