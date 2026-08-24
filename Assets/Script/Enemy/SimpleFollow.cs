using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SimpleFollow : MonoBehaviour, IEnemy
{
    [SerializeField] private Animator animator;
    [SerializeField] private Entity entity;
    private bool isAttacking = false;
    void Start()
    {
        StartCoroutine(Follow());
    }
    void Update()
    {
        if (!isAttacking && GridManager.Instance.GetDistance(GameManager.instance.playerEntity.GetComponent<Entity>().coords, entity.coords) <= 1)
        {
            StartCoroutine(Attacking());
        }
    }

    IEnumerator Follow()
    {
        while (true)
        {
            if (!isAttacking)
            {
                FollowLogic();
            }
            yield return new WaitForSeconds(GameManager.instance.fixedSecondRate);
        }
    }
    
    public void FollowLogic()
    {
        GridManager.Instance.FollowLogic(entity);
        GridManager.Instance.RotateEntityToTarget(entity, GameManager.instance.playerEntity.coords);
    }
    
    IEnumerator Attacking()
    {
        isAttacking = true;
        GridManager.Instance.RotateEntityToTarget(entity, GameManager.instance.playerEntity.coords);

        animator.SetBool("IsAttacking", true);
        Vector2Int targetTile = GameManager.instance.playerEntity.coords;
        List<Vector2Int> dangerZone = new List<Vector2Int> { targetTile };
        TargetingController.instance.ShowAttackWarning(dangerZone, 1.5f);
        yield return new WaitForSeconds(1.5f);


        MusicManager.Instance.PlayHandPokeSound(transform.position);

        float activeDuration = 0.3f;
        float timer = 0f;
        bool hasDealtDamage = false;
        GridManager.Instance.MoveEntity(entity, entity.coords, 1.5f);
        while (timer < activeDuration)
        {
            if (!hasDealtDamage && GameManager.instance.playerEntity.coords == targetTile)
            {
                GameManager.instance.playerEntity.TakeDamage(5);
                hasDealtDamage = true;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        animator.SetBool("IsAttacking", false);
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }
}
