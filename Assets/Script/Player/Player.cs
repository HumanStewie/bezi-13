using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Vector2Int currentPosition = Vector2Int.zero;
    [SerializeField] private float angleTransTime = 15f;
    [SerializeField] private float xRotationOffset = 0f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float hitboxRadius = 1.0f;
    
    private Quaternion targetRotation;
    private float initialCooldown;
    private bool canAttack = true;
    private Vector3 clickedVector;
    
    private void Start()
    {
        initialCooldown = attackCooldown;
        targetRotation = transform.rotation;
    }
    void Update()
    {
        if (!canAttack)
            attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0) canAttack = true;
        currentPosition = GetComponent<Entity>().coords;
        StartCoroutine(MovementDelay());
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * angleTransTime);
    }
    void Movement()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (GridManager.Instance.CheckTileExistence(currentPosition + new Vector2Int(0, 1)))
            {
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), currentPosition + new Vector2Int(0, 1));
                targetRotation = Quaternion.Euler(xRotationOffset, -90f, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (GridManager.Instance.CheckTileExistence(currentPosition + new Vector2Int(0, -1)))
            {
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), currentPosition + new Vector2Int(0, -1));
                targetRotation = Quaternion.Euler(xRotationOffset, 90, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (GridManager.Instance.CheckTileExistence(currentPosition + new Vector2Int(1, 0)))
            {
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), currentPosition + new Vector2Int(1, 0));
                targetRotation = Quaternion.Euler(xRotationOffset, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (GridManager.Instance.CheckTileExistence(currentPosition + new Vector2Int(-1, 0)))
            {
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), currentPosition + new Vector2Int(-1, 0));
                targetRotation = Quaternion.Euler(xRotationOffset, 180, 0);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            var node = GridManager.Instance.GetTileInMouse(Input.mousePosition);
            if (node)
            {
                Attack(node);
            }
        }
    }



    IEnumerator MovementDelay()
    {
        Movement();
        yield return new WaitForSeconds(0.07f);
    }

    void Attack(Node node)
    {
        canAttack = false;
        Vector3 nodeWorldPosition = new Vector3(node.gameObject.transform.position.x, 1, node.gameObject.transform.position.z);
        clickedVector = (nodeWorldPosition - transform.position).normalized * 2f;
        
        targetRotation = Quaternion.LookRotation(clickedVector, Vector3.up);
        Collider[]  colliders = Physics.OverlapSphere(clickedVector + Vector3.up * 2f + transform.position, hitboxRadius);
        
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out Entity entity))
            {
                if (entity.name != "Player")
                {
                    entity.TakeDamage(damage);
                }
            }
        }
        
        // TODO: Show particle
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(clickedVector+ Vector3.up * 2f + transform.position, hitboxRadius);
    }
}
