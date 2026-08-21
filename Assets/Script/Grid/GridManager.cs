using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2Int gridSize;
    
    public static GridManager Instance;
    private Dictionary<Vector3Int, Node> gridObjects = new Dictionary<Vector3Int, Node>();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        GenerateGrid();
    }
    
    void Start()
    {
        var worldPosition = grid.GetCellCenterWorld(new Vector3Int(0, 1));
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GenerateNode(x, y);
            }
        }
    }

    void GenerateNode(int x, int y)
    {
        
    }
}
