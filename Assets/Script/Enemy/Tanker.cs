using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : MonoBehaviour
{
    private Entity entity;

    void Start()
    {
        entity = this.GetComponent<Entity>();

        transform.SetParent(GridManager.Instance.transform);

        transform.localRotation = Quaternion.identity;

        GridManager.Instance.MoveEntity(entity, entity.coords, 1.8f);

        StartCoroutine(SpawnDropRoutine());
    }

    IEnumerator SpawnDropRoutine()
    {
        Vector3 groundedPos = transform.position;
        Vector3 skyPos = groundedPos + (GridManager.Instance.transform.up * 8f);

        transform.position = skyPos;

        float timer = 0f;
        float dropDuration = 0.4f;

        while (timer < dropDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / dropDuration;
            transform.position = Vector3.Lerp(skyPos, groundedPos, percent * percent);
            yield return null;
        }

        transform.position = groundedPos;

        // Heavy impact polish
        if (CameraShake.Instance != null) CameraShake.Instance.ShakeCamera(4f, 0.3f);
        if (MusicManager.Instance != null) MusicManager.Instance.PlayBlockPlaceSound(transform.position);

        StartCoroutine(BehaviorLoop()); 
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager.instance.fixedSecondRate * 1.5f);

            if (GameManager.instance != null && !GameManager.instance.gameOver)
            {
                GridManager.Instance.FollowLogicButNoDiagonal(entity);

                if (MusicManager.Instance != null) MusicManager.Instance.PlayMovementSound(transform.position);
            }
        }
    }
}