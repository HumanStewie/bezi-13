using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private Entity entity;
    public bool isHeld;
    private Rigidbody rb;
    void Start()
    {
        if (!isHeld)
        {
            transform.SetParent(GridManager.Instance.transform);

        }
        rb = GetComponent<Rigidbody>();
        GridManager.Instance.MoveEntity(entity, entity.coords);
    }

    private void FixedUpdate()
    {
        Mathf.Clamp(rb.linearVelocity.y, 0, 0.1f);
    }
    // void FixedUpdate()
    // {
    //     if (!isHeld)
    //     {
    //         Collider[] colliders = Physics.OverlapSphere(bottom.transform.position, 0.5f);
    //         if (colliders.Length > 0)
    //         {
    //             foreach (Collider col in colliders)
    //             {
    //                 if (col.TryGetComponent(out Node node) && GridManager.Instance.GetEntityAtPosition(node.cords) is null)
    //                 {
    //                     entity.coords = node.cords;
    //                     GridManager.Instance.RegisterEntity(entity);
    //                     GridManager.Instance.MoveEntity(entity, entity.coords, 1.5f);
    //                     transform.rotation = node.transform.rotation;
    //                 }
    //             }
    //         }
    //     }
    // }
}
