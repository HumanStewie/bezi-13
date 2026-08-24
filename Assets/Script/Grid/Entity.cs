using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;
    public Vector2Int coords;

    public float currentHealth;
    public float maxHealth = 100;
    public float weight = 1;

    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private GameObject PoofParticle;

    public Rigidbody Rigidbody => rb;
    private Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (entityName is not ("Player" or "Block"))
        {
            rb.isKinematic = true;
        }
        currentHealth = maxHealth;
        coords = GridManager.Instance.WorldToCoord(this.transform.position);
        transform.SetParent(GridManager.Instance.transform);
    }
    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;
        Instantiate(bloodParticle, transform.position, Quaternion.identity);
        if (entityName == "Player") MusicManager.Instance.PlayTakingDamageSound(transform.position);

    }
    void Die()
    {
        if (entityName != "Player")
        {
            GameManager.instance.enemyKillCount++;
        }
        MusicManager.Instance.PlayEnemyDieSound(transform.position);
        Destroy(gameObject);
    }
}
