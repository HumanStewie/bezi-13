using UnityEngine;
using System.Collections.Generic;
using Unity.Jobs;

public class TargetingController : MonoBehaviour
{
    public static TargetingController instance;

    private GridManager gridManager;
    public Entity playerEntity;

    private bool isTargeting = false;

    public List<Vector2Int> currentValidRange = new List<Vector2Int>();
    public List<Vector2Int> currentHoverSpread = new List<Vector2Int>();
    private List<Vector2Int> oldHoverSpread = new List<Vector2Int>();

    public Vector2Int currentHoveredCoord =new Vector2Int(-999, -999);
    private Vector2Int lastHoveredCoord;

    public Mesh ChangedMesh;


    public int placeholderRange = 0;

    private void Awake()
    {
        instance = this;
        gridManager = GridManager.Instance;
    }
    public void UpdateDragTargeting()
    {

        currentValidRange = GridManager.Instance.GetTilesInRange(playerEntity.coords, placeholderRange);

        foreach (Vector2Int coord in currentValidRange)
        {
            if (!currentHoverSpread.Contains(coord)) 
            gridManager.ChangeTileColor(coord, Color.white);
        }
        isTargeting = true;


        Vector2Int mouseCoord = GetCoordinateUnderMouse();
        if (mouseCoord != currentHoveredCoord)
        {
            oldHoverSpread = currentHoverSpread;
            foreach (Vector2Int coord in oldHoverSpread)
            {
                if (!currentValidRange.Contains(coord))
                gridManager.Grid[coord].ResetVisuals();
            }

            UpdateHoverHighlights(mouseCoord);
            currentHoveredCoord = mouseCoord;
        }
    }
    private void UpdateHoverHighlights(Vector2Int mouseCoord)
    {
        currentHoverSpread.Clear();

        if (currentValidRange.Contains(mouseCoord))
        {
            currentHoverSpread = gridManager.GetTilesInShape(playerEntity.coords, mouseCoord, TargetShape.Single, placeholderRange);

            foreach (Vector2Int coord in currentHoverSpread)
            {
                gridManager.ChangeTileColor(coord, Color.red);
            }
        }
    }
    public bool IsValidTarget(Vector2Int target)
    {
        return currentValidRange.Contains(target);
    }
    

    public void CancelTargeting()
    {
        isTargeting = false;

        currentValidRange.Clear();
        currentHoverSpread.Clear();
        gridManager.ResetAllTiles();
    }

    public Vector2Int GetCoordinateUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out Node tile))
            {
                return tile.cords;
            }
        }
        return new Vector2Int(-999, -999);
    }
}