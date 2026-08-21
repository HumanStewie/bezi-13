using System;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GridManagerDemo : MonoBehaviour
{
    public static GridManagerDemo Instance;
    public Vector2Int gridSize;
    [SerializeField] int tileSize;
    [SerializeField] private GameObject tilePrefab;
    public int TileSize { get { return tileSize; } }
    public Dictionary<Vector2Int, Node> Grid { get { return grid; } }

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    public string currentShape = "Grid";
    private void Awake()
    {
        if (Instance == null) Instance = this;
        GenerateStarGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GenerateNode(x, y);
            }
        }
        currentShape = "Grid";
    }
    private void GenerateStarGrid()
    {
        for (int x = 0; x <= gridSize.x; x++)
        {
            if (x <= gridSize.x / 2)
            {
                for (int y = -x; y <= x; y++)
                {
                    GenerateNode(x, y);
                }
            }
            else
            {
                for (int y = x - gridSize.x; y <= gridSize.x - x; y++)
                {
                    GenerateNode(x, y);
                }
            }
        }
        currentShape = "Star";
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


    public void TeleportPlayer(Vector2Int targetCoord)
    {
        GameObject player = GameObject.FindWithTag("Player");
        CharacterController cc = player.GetComponent<CharacterController>();

        Vector3 targetPos = CoordToWorldPos(targetCoord);   

        cc.enabled = false;

        player.transform.position = targetPos;
        var rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;  
            rb.angularVelocity = Vector3.zero;
        }
        cc.enabled = true;
    }


    public void FixNodePositions()
    {
        foreach (var kvp in grid)
        {
            Vector2Int coord = kvp.Key;
            Node node = kvp.Value;
            if (node == null) continue;
            node.transform.localPosition = new Vector3(coord.x * tileSize, 0f, coord.y * tileSize);
        }
    }

}