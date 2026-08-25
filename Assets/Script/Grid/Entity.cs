using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public string entityName;
    public Vector2Int coords;

    public float currentHealth;
    public float maxHealth = 100;
    public float weight = 1;

    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private GameObject PoofParticle;


    [SerializeField] private Image Filler;
    public Rigidbody Rigidbody => rb;
    private Rigidbody rb;

    public List<GameObject> indicators;



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
        if (Filler != null) {
            Filler.fillAmount = currentHealth / maxHealth;
        }
    }
    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;
        Instantiate(bloodParticle, transform.position, Quaternion.identity);
        if (entityName == "Player") MusicManager.Instance.PlayTakingDamageSound(transform.position);

    }

    private bool isDead = false;
    public void Die()
    {
        if (entityName != "Player")
        {
            GameManager.instance.enemyKillCount++;
            Instantiate(PoofParticle, transform.position, Quaternion.identity);
            MusicManager.Instance.PlayEnemyDieSound(transform.position);
            for (int i = 0; i < indicators.Count; i++) {
                Destroy(indicators[i]);
            }
            indicators.Clear();
            CameraShake.Instance.ShakeCamera(0.5f, 0.1f);
            Destroy(gameObject);
        }
        else
        {
            if (!isDead)
            {
                isDead = true;
                MusicManager.Instance.PlayGameOverSound();
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
                rb.AddTorque(new Vector3(-8f, 10f, 10f), ForceMode.Impulse);
                rb.AddForce(transform.forward, ForceMode.Impulse);
            }
        }
        
    }
}
