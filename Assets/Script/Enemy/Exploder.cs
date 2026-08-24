using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : MonoBehaviour
{
    bool isAttacking = false;
    Entity entity;
    [SerializeField] private float explosionTime;    
    
    void Start()
    {
        entity = GetComponent<Entity>();
        StartCoroutine(BehaviourLoop());
    }


    IEnumerator BehaviourLoop()
    {
        while (true)
        {
            if (!isAttacking)
            {
                FollowLogic();
            }
            yield return new WaitForSeconds(GameManager.instance.fixedSecondRate/2);
        }
    }

    void FollowLogic()
    {
        GridManager.Instance.FollowLogic(entity);
        if (GridManager.Instance.GetDistance(entity.coords, GameManager.instance.playerEntity.GetComponent<Entity>().coords) <= 2)
        {
            StartCoroutine(Attacking());
        }
    }
    IEnumerator Attacking()
    {
        isAttacking = true;
        List<Vector2Int> targetTiles = GridManager.Instance.GetTilesInRange(entity.coords, 3);
        targetTiles.Add(entity.coords);
        TargetingController.instance.ShowAttackWarning(targetTiles, 1f);

        Vector3 originalScale = this.transform.localScale;

        float timer = 0;
        float pulseSpeed = 25f;
        float pulseAmount = 0.3f;

        while (timer <= explosionTime)
        {
            float scaleMultiplier = 1f + (Mathf.Sin(timer * pulseSpeed) * pulseAmount);
            transform.localScale = originalScale * scaleMultiplier;

            timer += Time.deltaTime;

            yield return null;
        }

        timer = 0f;
        Vector3 currentScale = transform.localScale;
        Vector3 explodingScale = originalScale * 2f;
        while (timer < 0.3f)
        {
            float percentComplete = timer / 0.3f;
            transform.localScale = Vector3.Lerp(currentScale, explodingScale, percentComplete);

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var tile in targetTiles) {
            var Newentity = GridManager.Instance.GetEntityAtPosition(tile);
            if (Newentity)
            {
                Newentity.TakeDamage(10f);
            }
        }
        yield return null;
        MusicManager.Instance.PlayExplosionSound(transform.position);
        Destroy(this.gameObject);
    }
}
