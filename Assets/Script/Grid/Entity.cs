using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;
    public Vector2Int coords;

    public float currentHealth;
    public float maxHealth = 100;

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;
    }
    void Die()
    {
        Destroy(gameObject);
    }
}
