using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;
    public Vector2Int coords;

    public float currentHealth;
    public float maxHealth = 100;
    public float weight = 1;

    [SerializeField] private GameObject bloodParticle;
    private void Start()
    {
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
    }
    void Die()
    {
        GridManager.Instance.UnregisterEntity(this);
        if (entityName != "Player")
        {
            GameManager.instance.enemyKillCount++;
        } 
        Destroy(gameObject);
    }
}
