using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : MonoBehaviour
{
    private bool isAttacking = false;
    void Start()
    {
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
