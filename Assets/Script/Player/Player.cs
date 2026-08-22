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
    [SerializeField] private GameObject BlockPrefab;
    [SerializeField] private GameObject projectile;
    
    private void Start()
    {
        initialCooldown = attackCooldown;
        targetRotation = transform.rotation;
        transform.SetParent(GameManager.instance.transform);
        GridManager.Instance.RegisterEntity(GetComponent<Entity>());
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
                if (GetComponent<PlayerUpgrades>().CanBlock2)
                {
                    ThrowBlock(node);
                }
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
        Vector3 nodeWorldPosition = node.transform.position;
        nodeWorldPosition.y += 1f;
        clickedVector = (nodeWorldPosition - transform.position).normalized * 2f;
        
        targetRotation = Quaternion.LookRotation(clickedVector, Vector3.up);

        Vector3 forwardDirection = clickedVector.normalized;

        if (GetComponent<PlayerUpgrades>().canShoot)
        {

            Shoot(forwardDirection);
        }
        if (GetComponent<PlayerUpgrades>().canShoot2)
        {
            Shoot(-forwardDirection);
        }

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

    public void ResetAllBlock()
    {
        foreach (var block in Blocks)
        {
            Blocks.Remove(block);
            Destroy(block);
        }
    }

    public void SummonBlock()
    {
        GameObject block = Instantiate(BlockPrefab);
        block.transform.SetParent(this.transform);
        Blocks.Add(block.GetComponent<Entity>());
        block.transform.localPosition = new Vector3(0, 2, 0);
    }

    private void Shoot(Vector3 shootDirection)
    {
        if (projectile == null || shootDirection == Vector3.zero) return;

        Quaternion bulletRotation = Quaternion.LookRotation(shootDirection, Vector3.up);

        GameObject bullet = Instantiate(projectile, transform.position, bulletRotation);
        bullet.transform.SetParent(GridManager.Instance.transform, true);
    }

    private void ThrowBlock(Node node)
    {
        if (node.cords == currentPosition)
        {
            return;
        }
        if (Blocks.Count > 0)
        {
            StartCoroutine(ActualThrowBlock(node));
        }
    }

    IEnumerator ActualThrowBlock(Node node)
    {
        float timed = 0f;
        float throwDuration = 0.5f;
        float arcHeight = 3f;


        Blocks[0].transform.parent = null;
        Blocks[0].transform.position = GridManager.Instance.CoordToWorldPos(Blocks[0].coords) + new Vector3(0, 2, 0);
        GameObject block = Blocks[0].gameObject;
        Blocks.Remove(Blocks[0]);


        Vector3 nodeWorldPosition = new Vector3(node.gameObject.transform.position.x, 1, node.gameObject.transform.position.z);
        clickedVector = (nodeWorldPosition - transform.position).normalized * 2f;

        targetRotation = Quaternion.LookRotation(clickedVector, Vector3.up);


        while (timed < throwDuration)
        {
            if (block == null) yield break;

            timed += Time.deltaTime;
            float percent = timed / throwDuration;

            Vector3 currentPos = Vector3.Lerp(transform.position + new Vector3(0,2,0), GridManager.Instance.CoordToWorldPos(node.cords) + new Vector3(0,1,0), percent);

            currentPos.y += 4f * arcHeight * percent * (1f - percent);

            block.transform.position = currentPos;

            block.transform.Rotate(Vector3.right * 1000f * Time.deltaTime);

            yield return null;
        }
    }
}
