using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : MonoBehaviour
{
    private bool isAttacking = false;
    private Entity entity;
    void Start()
    {
        entity = this.GetComponent<Entity>();
        transform.parent = GridManager.Instance.transform;
        GridManager.Instance.MoveEntity(entity, entity.coords, 1.8f);
        StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            GridManager.Instance.FollowLogicButNoDiagonal(this.GetComponent<Entity>());
            yield return new WaitForSeconds(GameManager.instance.fixedSecondRate * 1.5f);
        }
    }
}
