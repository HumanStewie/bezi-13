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


    private void Start()
    {
        entity = GetComponent<Entity>();
        initialCooldown = attackCooldown;
        targetRotation = transform.rotation;
        transform.SetParent(GameManager.instance.transform);
        GridManager.Instance.RegisterEntity(entity);
        upgrades = GetComponent<PlayerUpgrades>();
    }
    void Update()
    {
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
        StartCoroutine(AttackLungeRoutine(forwardDirection));

        if (upgrades.canShoot)
        {

            Shoot(forwardDirection);
        }
        if (upgrades.canShoot2)
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
                    if (upgrades.WeightLessNess)
                    {
                        entity.GetComponent<Rigidbody>().mass = 0;
                    }
                }
            }
        }
        attackCooldown = initialCooldown;
        upgrades.AttackCounter += 1;
        GetComponent<PlayerUpgrades>().OnGenericAction();
    }

    private IEnumerator AttackLungeRoutine(Vector3 direction)
    {
        isAttacking = true;
        Vector3 basePos = GridManager.Instance.CoordToWorldPos(GetComponent<Entity>().coords);

        Vector3 boardUp = GridManager.Instance.transform.up;

        Vector3 flatDirection = Vector3.ProjectOnPlane(direction, boardUp).normalized;

        Quaternion baseRot = Quaternion.LookRotation(flatDirection, boardUp);

        float angleY = baseRot.eulerAngles.y;
        if (angleY > 180f)
        {
            angleY -= 360f;
        }

        float distanceMultiplier = 1f;

        if (angleY >= 0f && angleY <= 180f)
        {
            distanceMultiplier = 1f + (Mathf.Abs(angleY - 90f) / 90f) * 2f;
        }
        else
        {
            distanceMultiplier = 1f + (Mathf.Abs(angleY + 90f) / 90f) * 2f;
        }

        Vector3 lungeTarget = basePos + (flatDirection * distanceMultiplier) + (boardUp * 0.5f);

        Quaternion tiltRot = baseRot * Quaternion.Euler(-xRot, direction.y - dirYOffset, 0);

        float lungeSpeed = lungeTime;
        float elapsed = 0f;

        while (elapsed < lungeSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lungeSpeed;
            transform.position = Vector3.Lerp(basePos, lungeTarget, t);
            transform.rotation = Quaternion.Slerp(baseRot, tiltRot, t);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < lungeSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lungeSpeed;
            transform.position = Vector3.Lerp(lungeTarget, basePos, t);
            transform.rotation = Quaternion.Slerp(tiltRot, baseRot, t);
            yield return null;
        }

        transform.position = basePos;
        transform.rotation = baseRot;

        isAttacking = false;
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

            if (upgrades.ProfessionalHater)
            {
                Collider[] hitColliders = Physics.OverlapBox(currentPos, new Vector3(0.75f, 0.75f, 0.75f), Quaternion.identity);
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
    }
}
