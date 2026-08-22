using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour, IEnemy
{
    [SerializeField] private Animator animator;
    [SerializeField] private Entity entity;
    private bool isAttacking = false;
    void Start()
    {
        StartCoroutine(Follow());
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
        if (GridManager.Instance.GetDistance(GameManager.instance.playerEntity.GetComponent<Entity>().coords, entity.coords) <= 1)
        {
            StartCoroutine(Attacking());
        }
    }

    IEnumerator Attacking()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);
        Vector2Int targetTile = GameManager.instance.playerEntity.coords;
        List<Vector2Int> dangerZone = new List<Vector2Int> { targetTile };

        Vector3 lookAtTarget = GridManager.Instance.CoordToWorldPos(targetTile);
        lookAtTarget.y = transform.position.y;
        transform.LookAt(lookAtTarget);
        TargetingController.instance.ShowAttackWarning(dangerZone, 0.5f);
        yield return new WaitForSeconds(0.5f);

        float activeDuration = 0.6f;
        float timer = 0f;
        bool hasDealtDamage = false;

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
        isAttacking = false;
    }
}
