using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Vector2Int currentPosition = Vector2Int.zero;
    [SerializeField] private float angleTransTime = 15f;
    [SerializeField] private float xRotationOffset = 0f;
    public int damage = 1;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float hitboxRadius = 1.0f;
    
    private Quaternion targetRotation;
    private float initialCooldown;
    private bool canAttack = true;
    private Vector3 clickedVector;

    private Vector2Int latestLook = Vector2Int.zero;

    List<Entity> Blocks = new();
    
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
        Attack();




        currentPosition = GetComponent<Entity>().coords;

        StartCoroutine(MovementDelay());


        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * angleTransTime);


        if (Input.GetKeyDown(KeyCode.F))
        {
            BlockPlacement();
        }
    }
    void TryMove(Vector2Int direction, float yRotation)
    {
        Vector2Int targetPos = currentPosition + direction;

        if (!GridManager.Instance.CheckTileExistence(targetPos)) return;

        Entity entitys = GridManager.Instance.GetEntityAtPosition(targetPos);

        if (entitys != null && entitys.entityName == "Block")
        {
            Transform block = entitys.transform;
            block.SetParent(this.transform);
            Blocks.Add(block.GetComponent<Entity>());
            block.localPosition = new Vector3(0, 2, 0);
        }

        GetComponent<PlayerUpgrades>().OnPlayerMoved(this.GetComponent<Entity>().coords);
        GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), targetPos);
        targetRotation = Quaternion.Euler(xRotationOffset, yRotation, 0);
        latestLook = direction;
    }
    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
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
        if (Input.GetKeyDown(KeyCode.W)) TryMove(new Vector2Int(0, 1), -90f);
        else if (Input.GetKeyDown(KeyCode.S)) TryMove(new Vector2Int(0, -1), 90f);
        else if (Input.GetKeyDown(KeyCode.D)) TryMove(new Vector2Int(1, 0), 0f);
        else if (Input.GetKeyDown(KeyCode.A)) TryMove(new Vector2Int(-1, 0), 180f);
        yield return new WaitForSeconds(0.07f);
    }

    void Attack(Node node)
    {
        if (node.cords == currentPosition)
        {
            return;
        }
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
        attackCooldown = initialCooldown;
        // TODO: Show particle
        GetComponent<PlayerUpgrades>().AttackCounter += 1;
    }

    void BlockPlacement()
    {
        Vector2Int PlaceLocation = this.GetComponent<Entity>().coords += latestLook;
        if (Blocks.Count > 0)
        {
            Blocks[0].transform.parent = null;
            Blocks[0].coords = PlaceLocation;
            Blocks[0].transform.rotation = Quaternion.identity;
            Blocks[0].transform.position = GridManager.Instance.CoordToWorldPos(Blocks[0].coords) + new Vector3(0,2,0);
            Blocks.Remove(Blocks[0]);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(clickedVector+ Vector3.up * 2f + transform.position, hitboxRadius);
    }


    private void Shoot(Vector2Int direction)
    {

    }
}
