using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

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
    [SerializeField] int tileSize;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject pivotPoint;
    public int TileSize { get { return tileSize; } }
    public Dictionary<Vector2Int, Node> Grid { get { return grid; } }

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        GenerateGrid();
    }
    private void Update()
    {
        float currentTilt = Vector3.Angle(Vector3.up, transform.up);

        if (currentTilt >= 35f)
        {
            transform.rotation = Quaternion.identity;
            GameObject.FindWithTag("Player").transform.position = new Vector3(0, 1, 0);
        }
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

        Instantiate(pivotPoint, new Vector3(centerX, -1, centerY), Quaternion.identity);
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
        return transform.position + new Vector3(coord.x * tileSize, yOffset, coord.y * tileSize);
    }

    public Vector2Int WorldToCoord(Vector3 worldPos)
    {
        Vector3 local = worldPos - transform.position;
        int x = Mathf.RoundToInt(local.x / tileSize);
        int y = Mathf.RoundToInt(local.z / tileSize);
        return new Vector2Int(x, y);
    }


    public void MoveEntity(Entity entity, Vector2Int targetCoord)
    {
        if (!grid.ContainsKey(targetCoord)) return;

        Vector3 newPos = CoordToWorldPos(targetCoord);

        newPos.y = GetExactPlatformHeight(newPos) + 1.5f;

        entity.transform.position = newPos;

        entity.coords = targetCoord;
    }

    public float GetExactPlatformHeight(Vector3 targetPosition)
    {

        Vector3 rayStartPoint = new Vector3(targetPosition.x, targetPosition.y + 10f, targetPosition.z);

        if (Physics.SphereCast(rayStartPoint, 0.5f ,Vector3.down, out RaycastHit hit, 20f))
        {
            return hit.point.y;
        }

        return targetPosition.y;
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
}