using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Vector2Int currentPosition = Vector2Int.zero;
    [SerializeField] private float angleTransTime = 15f;
    [SerializeField] private float xRotationOffset = 0f;
    public int damage = 5;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private float hitboxRadius = 1.0f;
    [SerializeField] private Animator animator;
    
    private Quaternion targetRotation;
    private float initialCooldown;
    private bool canAttack = true;
    private bool isAttacking = false;
    private Vector3 clickedVector;

    private Vector2Int latestLook = Vector2Int.zero;

    List<Entity> Blocks = new();
    [SerializeField] private GameObject BlockPrefab;
    [SerializeField] private GameObject projectile;

    private PlayerUpgrades upgrades;
    private Entity entity;

    public float WRotation = -90f;
    public float SRotation = 90f;
    public float ARotation = 180f;
    public float DRotation = 0f;



    [Header("Attack")]
    public float dirYOffset = 150f;
    public float lungeTime = 0.1f;
    public float xRot = 35f;

    public Rigidbody Rigidbody => rb;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        entity = GetComponent<Entity>();
        initialCooldown = attackCooldown;
        targetRotation = transform.rotation;
        transform.SetParent(GameManager.instance.transform);
        GridManager.Instance.RegisterEntity(entity);
        upgrades = GetComponent<PlayerUpgrades>();
    }
    void Update()
    {
        if (GameManager.instance.gameOver) return;
        if (!canAttack)
            attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0) canAttack = true;
        Attack();

        if (!isAttacking)
        {
            Attack();
            StartCoroutine(MovementDelay());

            if (Input.GetKeyDown(KeyCode.F))
            {
                BlockPlacement();
            }
        }


        currentPosition = entity.coords;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * angleTransTime);

    }

    void TryMove(Vector2Int direction, float yRotation)
    {
        Vector2Int targetPos = currentPosition + direction;

        if (!GridManager.Instance.CheckTileExistence(targetPos))
        {
            Debug.Log("No move, in TryMove");
            return;
        }

        Node targetNode = GridManager.Instance.Grid.GetValueOrDefault(targetPos);
        
        Collider[] colliders = Physics.OverlapBox(targetNode.transform.position, new Vector3(0.5f, 50f, 0.5f), Quaternion.FromToRotation(Vector3.up, GridManager.Instance.transform.up));
        if (colliders.Length > 0)
        {
            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out Entity e) && e.entityName is not ("Block" or "Player"))
                {
                    Debug.Log("No move, in loop");
                    return;
                }
                if (!col.TryGetComponent(out Block b)) continue;
                if (b.isHeld) continue;
                Transform block = e.transform;
                block.SetParent(this.transform);
                Blocks.Add(e);
                b.isHeld = true;
                block.localPosition = new Vector3(0, Blocks.Count * 2, 0);
                entity.weight += e.weight;
            }
        }

        upgrades.OnPlayerMoved(this.entity.coords);
        GridManager.Instance.MoveEntity(this.entity, targetPos);
        if (Blocks.Count > 0)
        {
            foreach (var b in Blocks)
            {
                b.coords = entity.coords;
            }
        }
        targetRotation = Quaternion.Euler(xRotationOffset, yRotation, 0);
        latestLook = direction;
        Debug.Log(Blocks.Count);
    }
    
    void BlockPlacement()
    {
        Vector2Int placeLocation = this.entity.coords + latestLook;
        Node nodeToPlace = GridManager.Instance.Grid.GetValueOrDefault(placeLocation);
        if (!nodeToPlace) return;
        Vector3 spawnPos = nodeToPlace.transform.position;
        spawnPos += nodeToPlace.transform.up * 1.6f;
        if (Blocks.Count > 0)
        {
            Entity lastBlock = Blocks[Blocks.Count - 1];
            entity.weight -= lastBlock.weight;
            lastBlock.transform.SetParent(GridManager.Instance.transform);
            lastBlock.coords = placeLocation;
            lastBlock.transform.rotation = nodeToPlace.transform.rotation;
            lastBlock.transform.position = spawnPos;
            lastBlock.GetComponent<Block>().isHeld = false;
            GridManager.Instance.RegisterEntity(lastBlock);
            
            Blocks.Remove(lastBlock);
        }
        Debug.Log(Blocks.Count);
    }
    

    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            var node = GridManager.Instance.GetTileInMouse(Input.mousePosition);
            if (node)
            {
                Debug.Log("yessss");
                Attack(node);
                if (upgrades.CanBlock2)
                {
                    ThrowBlock(node);
                }
            }
        }
    }


    IEnumerator MovementDelay()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            TryMove(new Vector2Int(0, 1), WRotation);
        }
        else if (Input.GetKeyDown(KeyCode.S)) TryMove(new Vector2Int(0, -1), SRotation);
        else if (Input.GetKeyDown(KeyCode.D)) TryMove(new Vector2Int(1, 0), DRotation);
        else if (Input.GetKeyDown(KeyCode.A)) TryMove(new Vector2Int(-1, 0), ARotation);
        yield return new WaitForSeconds(0.07f);
    }

    void Attack(Node node)
    {
        if (node.cords == currentPosition) return;

        canAttack = false;

        Vector3 boardUp = GridManager.Instance.transform.up;

        Vector3 nodeWorldPosition = node.transform.position;
        nodeWorldPosition += boardUp * 1f; 

        clickedVector = (nodeWorldPosition - transform.position).normalized * 2f;

        Vector3 flatDirection = Vector3.ProjectOnPlane(clickedVector, boardUp);
        if (flatDirection != Vector3.zero)
        if (flatDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(flatDirection, boardUp);

            transform.rotation = targetRotation;
        }

        Vector3 forwardDirection = transform.forward;

        StartCoroutine(AttackLungeRoutine(forwardDirection));

        if (upgrades.canShoot) { Shoot(forwardDirection); }
        if (upgrades.canShoot2) { Shoot(-forwardDirection); }

        Collider[] colliders = Physics.OverlapSphere(transform.position + (forwardDirection * 1f) + (boardUp * 1f), hitboxRadius);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out Entity entity))
            {
                if (entity.name != "Player")
                {
                    entity.TakeDamage(damage);
                    if (upgrades.WeightLessNess)
                    {
                        entity.weight = 0;
                    }
                }
            }
        }

        attackCooldown = initialCooldown;
        upgrades.AttackCounter += 1;
        GetComponent<PlayerUpgrades>().OnGenericAction();
        animator.SetBool("IsAttacking", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(clickedVector + Vector3.up * 1f + transform.position, 1.0f);
    }

    private IEnumerator AttackLungeRoutine(Vector3 direction)
    {
        isAttacking = true;
        float lungeSpeed = lungeTime;
        float elapsed = 0f;
        animator.SetBool("IsAttacking", isAttacking);
        animator.Play("DominoAttack");
        while (elapsed < lungeSpeed * 2)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isAttacking = false;
        animator.SetBool("IsAttacking", isAttacking);
    }


    public void ResetAllBlock()
    {
        for (int i = Blocks.Count - 1; i >= 0; i--)
        {
            Entity block = Blocks[i];

            this.entity.weight -= block.weight;

            Destroy(block.gameObject);
        }

        Blocks.Clear();
    }

    public void SummonBlock()
    {
        GameObject blockObj = Instantiate(BlockPrefab);

        Entity blockEntity = blockObj.GetComponent<Entity>();
        Blocks.Add(blockEntity);

        blockObj.GetComponent<Block>().isHeld = true;
        blockEntity.coords = this.entity.coords;
        this.entity.weight += blockEntity.weight;

        StartCoroutine(ForceParentAtEndOfFrame(blockObj, Blocks.Count));
    }

    private IEnumerator ForceParentAtEndOfFrame(GameObject blockObj, int stackCount)
    {
        yield return new WaitForEndOfFrame();

        if (blockObj != null)
        {
            blockObj.transform.SetParent(this.transform);

            blockObj.transform.localRotation = Quaternion.identity;

            Vector3 currentPos = blockObj.transform.localPosition;
            currentPos.y = stackCount * 2f;
            currentPos.x = 0f;
            currentPos.z = 0f;
            blockObj.transform.localPosition = currentPos;
        }
    }

    private void Shoot(Vector3 shootDirection)
    {
        if (projectile == null || shootDirection == Vector3.zero) return;

        Vector3 boardUp = GridManager.Instance.transform.up;

        Quaternion bulletRotation = Quaternion.LookRotation(shootDirection, boardUp);

        Vector3 spawnPos = transform.position + (boardUp * 0.5f) + (shootDirection * 0.5f);

        GameObject bullet = Instantiate(projectile, spawnPos, bulletRotation, GridManager.Instance.transform);

        if (bullet.TryGetComponent(out Bullet bulletScript))
        {
            bulletScript.player = true;
        }
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

        Entity thrownBlockEntity = Blocks[0];
        GameObject blockObj = thrownBlockEntity.gameObject;
        Blocks.Remove(thrownBlockEntity);

        this.entity.weight -= thrownBlockEntity.weight;

        blockObj.transform.SetParent(GridManager.Instance.transform);

        Vector3 boardUp = GridManager.Instance.transform.up;
        Vector3 rawDirection = node.transform.position - transform.position;
        Vector3 flatDirection = Vector3.ProjectOnPlane(rawDirection, boardUp);
        if (flatDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(flatDirection, boardUp);
        }

        Vector3 startPos = transform.position + (boardUp * 2f); 
        Vector3 endPos = node.transform.position + (boardUp * 1f); 

        while (timed < throwDuration)
        {
            if (blockObj == null) yield break;

            timed += Time.deltaTime;
            float percent = timed / throwDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, percent);

            float currentArc = 4f * arcHeight * percent * (1f - percent);
            currentPos += boardUp * currentArc;

            blockObj.transform.position = currentPos;

            blockObj.transform.Rotate(Vector3.right * 1000f * Time.deltaTime, Space.Self);

            if (upgrades.ProfessionalHater)
            {
                Collider[] hitColliders = Physics.OverlapBox(currentPos, new Vector3(0.75f, 0.75f, 0.75f), GridManager.Instance.transform.rotation);
                foreach (var col in hitColliders)
                {
                    if (col.TryGetComponent(out Entity enemy) && enemy.name != "Player")
                    {
                        enemy.TakeDamage(9999);
                    }
                }
            }
            yield return null;
        }

        if (blockObj != null)
        {
            blockObj.transform.position = endPos;
            blockObj.transform.rotation = node.transform.rotation;

            thrownBlockEntity.coords = node.cords;
            thrownBlockEntity.GetComponent<Block>().isHeld = false;
            GridManager.Instance.RegisterEntity(thrownBlockEntity);
        }
    }
}
