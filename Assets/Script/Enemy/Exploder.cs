using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : MonoBehaviour
{
    bool isAttacking = false;
    Entity entity;
    [SerializeField] private float explosionTime;
    [SerializeField] private float explosionIntensity;
    [SerializeField] private float shakeDuration;

    bool isSpawning = true;

    void Start()
    {
        entity = GetComponent<Entity>();

        transform.SetParent(GridManager.Instance.transform);

        transform.localRotation = Quaternion.identity;

        GridManager.Instance.MoveEntity(entity, entity.coords, 1.5f);

        StartCoroutine(SpawnRiseRoutine());
    }

    IEnumerator SpawnRiseRoutine()
    {
        Vector3 finalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float percent = Mathf.SmoothStep(0f, 1f, timer / duration);
            transform.localScale = finalScale * percent;
            yield return null;
        }

        transform.localScale = finalScale;

        isSpawning = false;
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
        transform.rotation= Quaternion.identity;
        if (GridManager.Instance.GetDistance(entity.coords, GameManager.instance.playerEntity.GetComponent<Entity>().coords) <= 2 && !isSpawning)
        {
            StartCoroutine(Attacking());
        }
    }
    IEnumerator Attacking()
    {
        isAttacking = true;
        List<Vector2Int> targetTiles = GridManager.Instance.GetTilesInRange(entity.coords, 3);
        targetTiles.Add(entity.coords);
        TargetingController.instance.ShowAttackWarning(GetComponent<Entity>(), targetTiles, explosionTime + 0.2f);

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
        while (timer < 0.2f)
        {
            float percentComplete = timer / 0.2f;
            transform.localScale = Vector3.Lerp(currentScale, explodingScale, percentComplete);

            timer += Time.deltaTime;
            yield return null;
        }

        foreach (var tile in targetTiles) {
            var Newentity = GridManager.Instance.GetEntityAtPosition(tile);
            if (Newentity)
            {
                Newentity.TakeDamage(10f);
            }
        }
        yield return null;
        MusicManager.Instance.PlayExplosionSound(transform.position);
        CameraShake.Instance.ShakeCamera(explosionIntensity, shakeDuration);
        this.GetComponent<Entity>().Die();
    }
}
