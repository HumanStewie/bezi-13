using UnityEngine;

public class SpinningBall : MonoBehaviour
{
    [Header("Orbit Settings")]
    public float orbitRadius = 3f;
    public float spinSpeed = 200f;
    public int damage = 3;

    private Transform targetPlayer;
    private float currentAngle;

    public void Initialize(Transform target, float startingAngleDegrees)
    {
        targetPlayer = target;
        currentAngle = startingAngleDegrees;
    }
    private void Start()
    {
        Initialize(GameManager.instance.playerEntity.transform, 0);
    }
    void Update()
    {
        if (targetPlayer == null) return;

        currentAngle += spinSpeed * Time.deltaTime;

        if (currentAngle > 360f) currentAngle -= 360f;

        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * orbitRadius;

        transform.position = targetPlayer.position + offset + Vector3.up * 1f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            if (entity.name != "Player")
            {
                entity.TakeDamage(damage);
                Debug.Log("Spinning Ball dealt 3 damage!");
            }
        }
    }
}