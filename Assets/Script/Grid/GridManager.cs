using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum TargetShape
{
    Single,
    AoE,
    PiercingLine,
    All,
    Self

}
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public Vector2Int gridSize;
    [Header("Grid Setttings")]
    [SerializeField] float tileSize;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float yMoveOffset = 0.5f;
    public float gridWeight = 5f;
    [Header("References")]
    [SerializeField] private Camera gameCamera;         // Camera rendering to the RenderTexture
    [SerializeField] private RawImage displayRawImage;  // UI RawImage displaying the RenderTexture
    [SerializeField] private Canvas canvas;             // Canvas holding the RawImage
    [SerializeField] private TippingLogic tippingLogic;
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask hitLayers = ~0;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private bool snapToPixelGrid = false;

    private RectTransform rawImageRect;
    private Camera uiCamera;

    private float TileSize => tileSize; 
    public Dictionary<Vector2Int, Node> Grid { get { return grid; } }
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    
    public Dictionary<Vector2Int, HashSet<Entity>> entities = new Dictionary<Vector2Int, HashSet<Entity>>();
    public Rigidbody Rigidbody => rb;
    private Rigidbody rb;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        GenerateGrid();
        rawImageRect = displayRawImage.rectTransform;
        // If Canvas is Screen Space - Overlay, uiCamera must be null
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public float GetCurrentTilt()
    {
        return Vector3.Angle(Vector3.up, transform.up);
    }
    private void GenerateGrid()
    {
        float radius = gridSize.x / 2f;

        float centerX = 0;
        float centerY = 0;

        for (int x = -gridSize.x / 2; x <= gridSize.x/2; x++)
        {
            for (int y = -gridSize.y / 2; y <= gridSize.y/2; y++)
            {
                float distanceX = x - centerX;
                float distanceY = y - centerY;

                float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);

                if (distanceSquared <= (radius * radius))
                {
                    GenerateNode(x, y);
                }
            }
        }
    }

    public void ApplyRandomForceToAllEntities()
    {
        foreach (var entity in FindObjectsByType<Entity>(FindObjectsSortMode.None))
        {
            Vector3 randomForce = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
            if (entity.entityName == "Player")
            {
                entity.Rigidbody.isKinematic = false;
                entity.Rigidbody.constraints = RigidbodyConstraints.None;
                entity.Rigidbody.AddTorque(new Vector3(-3f, 3f, 5f), ForceMode.Impulse);
                entity.Rigidbody.AddForce(Vector3.down, ForceMode.Impulse);
            }

            entity.Rigidbody.constraints = RigidbodyConstraints.None;
            entity.Rigidbody.detectCollisions = true;
            entity.Rigidbody.isKinematic = false;
            entity.Rigidbody.AddForce(randomForce, ForceMode.Impulse);
            entity.Rigidbody.AddTorque(randomForce, ForceMode.Impulse);
            
        }
    }

    private void GenerateNode(int x, int y)
    {
        Vector2Int cords = new Vector2Int(x, y);

        Vector3 worldPos = new Vector3(x * TileSize, 0, y * TileSize);

        GameObject tileObj;

        tileObj = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
        tileObj.name = $"Tile {x} + {y}";


        Node node = tileObj.GetComponent<Node>();
        if (node == null)
        {
            node = tileObj.AddComponent<Node>();
        }
        node.Initialize(cords);
        grid.Add(cords, node);
    }

    public int GetDistance(Vector2Int x, Vector2Int y)
    {
        return Mathf.Abs(x.x - y.x) + Mathf.Abs(x.y - y.y);
    }

    public List<Vector2Int> GetTilesInRange(Vector2Int origin, int range)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (var coord in grid.Keys)
        {
            float distance = Vector2Int.Distance(origin, coord);
            if (distance <= range && distance > 0)
                result.Add(coord);
        }
        return result;
    }

    public List<Vector2Int> GetTilesInRangeIncludesSquareRoot(Vector2Int origin, int range)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (var coord in grid.Keys)
        {
            float distance = Vector2Int.Distance(origin, coord);
            if (distance <= range - 1 + Mathf.Sqrt(2) && distance > 0)
                result.Add(coord);
        }
        return result;
    }
    public List<Vector2Int> GetTilesInShape(Vector2Int origin, Vector2Int mouseCoord, TargetShape targetShape, int radius)
    {
        List<Vector2Int> inRangeTiles = new List<Vector2Int>();

        switch (targetShape)
        {
            case TargetShape.Single:
                if (grid.ContainsKey(mouseCoord))
                {
                    inRangeTiles.Add(mouseCoord);
                }

                break;
            case TargetShape.AoE:
                inRangeTiles.AddRange(GetTilesInRange(mouseCoord, radius));
                inRangeTiles.Add(mouseCoord);
                break;
            case TargetShape.PiercingLine:
                if (origin.x != mouseCoord.x)
                {
                    foreach (var key in Grid.Keys)
                    {
                        if (key.y == origin.y && (mouseCoord.x > origin.x) ? key.x > origin.x : key.x < origin.x)
                        {
                            inRangeTiles.Add(key);
                        }
                    }
                }
                else
                {
                    foreach (var key in Grid.Keys)
                    {
                        if (key.x == origin.x && (mouseCoord.y > origin.y) ? key.y > origin.y : key.y < origin.y)
                        {
                            inRangeTiles.Add(key);
                        }
                    }
                }
                break;
            case TargetShape.All:
                inRangeTiles.AddRange(grid.Keys);
                break;
            case TargetShape.Self:
                inRangeTiles.Add(origin);
                break;
            default:
                inRangeTiles.Add(mouseCoord);
                break;
        }
        return inRangeTiles;
    }

    /// <summary>
    /// Take raw mouse screen pos, then output a tile at mouse position
    /// </summary>
    /// <param name="mouseCoord"></param>
    /// <returns></returns>
    public Node GetTileInMouse(Vector3 mouseCoord)
    {
        
        if (TryGetWorldRay(out Ray ray))
        {
            if (Physics.Raycast(ray, out RaycastHit rayHit))
            {
                if (rayHit.collider.TryGetComponent<Node>(out Node node))
                {
                    return node;
                }
                else if (rayHit.collider.TryGetComponent<Entity>(out Entity entity))
                {
                    return grid.GetValueOrDefault(entity.coords);
                }
            }
        }

        return null;
    }
    private bool TryGetWorldRay(out Ray ray)
    {
        ray = default;
        Vector2 mousePos = Input.mousePosition;

        // 1. Convert screen position to local point inside the RawImage RectTransform
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rawImageRect, 
                mousePos, 
                uiCamera, 
                out Vector2 localPoint))
        {
            return false;
        }

        // 2. Convert local point to normalized UV coordinates (0.0 to 1.0)
        Rect rect = rawImageRect.rect;
        float u = (localPoint.x - rect.xMin) / rect.width;
        float v = (localPoint.y - rect.yMin) / rect.height;

        // 3. Reject if the cursor is outside the RawImage bounds (e.g. letterbox bars)
        if (u < 0f || u > 1f || v < 0f || v > 1f)
        {
            return false;
        }

        // 4. (Optional) Snap UV to low-res pixel grid
        if (snapToPixelGrid && displayRawImage.texture != null)
        {
            int texWidth = displayRawImage.texture.width;
            int texHeight = displayRawImage.texture.height;

            u = Mathf.Floor(u * texWidth) / texWidth;
            v = Mathf.Floor(v * texHeight) / texHeight;
        }

        // 5. Generate ray from the game camera using Viewport coordinates
        ray = gameCamera.ViewportPointToRay(new Vector3(u, v, 0f));
        return true;
    }
    
    public List<Vector2Int> GetNodeByX(int x)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var kvp in grid)
        {
            if (kvp.Key.x == x)
            {
                result.Add(kvp.Value.cords);
            }
        }
        return result;
    }

    public List<Vector2Int> GetNodeByY(int x)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var kvp in grid)
        {
            if (kvp.Key.y == x)
            {
                result.Add(kvp.Value.cords);
            }
        }
        return result;
    }

    public Entity GetEntityAtPosition(Vector2Int coord)
    {
        Entity[] allEntities = FindObjectsByType<Entity>(FindObjectsSortMode.None);

        foreach (Entity entity in allEntities)
        {
            if (entity.coords == coord)
                return entity;
        }

        return null;
    }
    public bool CheckTileExistence(Vector2Int coords)
    {
        foreach(var grids in grid.Keys)
        {
            if (grids == coords)
            {
                return true;
            }
        }
        return false;
    }
    


    public void ChangeTileColor(Vector2Int coord, Color color)
    {
        if (grid.TryGetValue(coord, out Node node))
            node.Highlight(color);
    }

    public void ResetAllTiles()
    {
        foreach (var node in grid.Values)
            node.ResetVisuals();
    }
    public Vector3 CoordToWorldPos(Vector2Int coord, float yOffset = 0.5f)
    {
        Vector3 worldPos = new Vector3(coord.x * tileSize, 0, coord.y * tileSize);
        
        return transform.position + worldPos;
    }

    public Vector2Int WorldToCoord(Vector3 worldPos)
    {
        Vector3 local = worldPos - transform.position;
        int x = Mathf.RoundToInt(local.x / tileSize);
        int y = Mathf.RoundToInt(local.z / tileSize);
        return new Vector2Int(x, y);
    }

    public void RegisterEntity(Entity entity)
    {
        if (!entities.TryGetValue(entity.coords, out var set))
        {
            set = new HashSet<Entity>();
            entities.Add(entity.coords, set);
        }
        set.Add(entity);
    }

    public void UnregisterEntity(Entity entity)
    {
        if (!entities.TryGetValue(entity.coords, out var set)) return;
        set.Remove(entity);
        if (set.Count == 0) entities.Remove(entity.coords);
    }
    
    public void MoveEntity(Entity entity, Vector2Int targetCoord, float yOffset = 1.0f)
    {
        if (!grid.ContainsKey(targetCoord))
        {
            Debug.Log("No move, in MoveEntity");
            return;
        }
        // if (entities.TryGetValue(targetCoord, out var set) && set.Count > 0) return; // If there's someone there already, stop
        UnregisterEntity(entity);
        Node nodeToMove = grid.GetValueOrDefault(targetCoord);
        Vector3 newPos = nodeToMove.transform.position;
        newPos += nodeToMove.transform.up * yOffset;
        entity.transform.position = newPos;
        entity.coords = targetCoord;
        RegisterEntity(entity);
    }
    
    /// <summary>
    /// Rotate to the target, regardless of rotation. Compensated for rotation
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="targetCoord"></param>
    public void RotateEntityToTarget(Entity entity, Vector2Int targetCoord)
    {
        Node nodeToRotateTo = grid.GetValueOrDefault(targetCoord);
        Vector3 target = nodeToRotateTo.transform.position;
        target += nodeToRotateTo.transform.up * 1.5f;
        entity.transform.LookAt(target);
    }
    public void RotateEntityToTargetWithY(Transform objectToRotate, Vector3 targetWorldPos)
    {
        Vector3 boardUp = transform.up;
        Vector3 rawDirection = targetWorldPos - objectToRotate.position;

        Vector3 flatDirection = Vector3.ProjectOnPlane(rawDirection, boardUp);

        if (flatDirection != Vector3.zero)
        {
            objectToRotate.rotation = Quaternion.LookRotation(flatDirection, boardUp);
        }
    }

    public float GetExactPlatformHeight(Vector3 targetPosition)
    {

        Vector3 rayStartPoint = new Vector3(targetPosition.x, targetPosition.y + 10f, targetPosition.z);

        if (Physics.SphereCast(rayStartPoint, 0.5f ,Vector3.down, out RaycastHit hit, 20f))
        {
            
            return hit.collider.transform.position.y;
        }
        
        return targetPosition.y;
    }

    private Vector3 rayStartPoint;
    public Node GetNodeBelowFeet(Vector3 targetPosition)
    {

        rayStartPoint = new Vector3(targetPosition.x, targetPosition.y + 10f, targetPosition.z);
        if (Physics.SphereCast(rayStartPoint, 0.5f ,Vector3.down, out RaycastHit hit, 20f))
        {
            return hit.collider.gameObject.GetComponent<Node>();
        }

        return grid.GetValueOrDefault(WorldToCoord(targetPosition));
    }


    public Vector2Int SelectRandomPossible()
    {
        Entity[] allEntities = GameObject.FindObjectsByType<Entity>(FindObjectsSortMode.None);

        HashSet<Vector2Int> occupiedCoords = new HashSet<Vector2Int>();
        foreach (Entity entity in allEntities)
        {
            occupiedCoords.Add(entity.coords);
        }

        List<Vector2Int> freeCoords = new List<Vector2Int>();
        foreach (Vector2Int gridCoord in GridManager.Instance.Grid.Keys)
        {
            if (!occupiedCoords.Contains(gridCoord))
            {
                freeCoords.Add(gridCoord);
            }
        }

        if (freeCoords.Count == 0)
        {
            return new Vector2Int(0, 0);
        }

        int randomIndex = UnityEngine.Random.Range(0, freeCoords.Count);
        return freeCoords[randomIndex];
    }



    //Enemy Follow Logic
    public void FollowLogic(Entity entity)
    {
        Entity PlayerEntity = GameManager.instance.playerEntity;
        Vector2Int playerPos = PlayerEntity.coords;
        Vector2Int bestCoord = entity.coords;
        float distance = 50;

        if (PlayerEntity != null)
        {
            List<Vector2Int> possibleTile = GetTilesInRangeIncludesSquareRoot(entity.coords, 1);
            foreach (var tile in possibleTile)
            {
                if (GetDistance(playerPos, tile) < distance && GetEntityAtPosition(tile) == null)
                {
                    bestCoord = tile;
                    distance = GetDistance(playerPos, tile);
                }

                if (GetDistance(playerPos, tile) == 0) return;
            }

            if (bestCoord != entity.coords && bestCoord != playerPos)
            {

                Vector3 lookAtTarget = CoordToWorldPos(bestCoord);
                lookAtTarget.y = transform.position.y + 1;
                entity.transform.LookAt(lookAtTarget);

                MoveEntity(entity, bestCoord, 1.5f);
            }
        }
    }

    public void FollowLogicButNoDiagonal(Entity entity)
    {
        Entity PlayerEntity = GameManager.instance.playerEntity;
        Vector2Int playerPos = PlayerEntity.coords;
        Vector2Int bestCoord = entity.coords;
        float distance = 50;

        if (PlayerEntity != null)
        {
            List<Vector2Int> possibleTile = GetTilesInRange(entity.coords, 1);
            foreach (var tile in possibleTile)
            {
                if (GetDistance(playerPos, tile) < distance && GetEntityAtPosition(tile) == null)
                {
                    bestCoord = tile;
                    distance = GetDistance(playerPos, tile);
                }

                if (GetDistance(playerPos, tile) == 0) return;
            }

            if (bestCoord != entity.coords && bestCoord != playerPos)
            {

                Vector3 lookAtTarget = CoordToWorldPos(bestCoord);
                lookAtTarget.y = transform.position.y + 1;
                entity.transform.LookAt(lookAtTarget);

                MoveEntity(entity, bestCoord, 1.8f);
            }
            RotateEntityToTargetWithY(entity.transform, CoordToWorldPos(bestCoord));
        }
    }
}