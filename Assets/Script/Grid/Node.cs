using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
    public Vector2Int cords;

    private Mesh startSprite;

    private Color originalColor;
    private Renderer tileRenderer;
    private MeshFilter filter;

    public void Start()
    {
        startSprite = this.GetComponent<MeshFilter>().mesh;
        tileRenderer = GetComponent<Renderer>();
        filter = GetComponent<MeshFilter>();

        if (tileRenderer != null && tileRenderer.material != null)
        {
            originalColor = tileRenderer.material.color;
        }
    }
    public Node(Vector2Int cords)
    {
        this.cords = cords;
    }
    public void Initialize(Vector2Int gridcoords)
    {
        cords = gridcoords;
    }

    public void ChangeMesh(Mesh newMesh)
    {
        filter.mesh = newMesh;
    }
    public void ResetVisuals()
    {
        filter.mesh = startSprite;
        tileRenderer.material.color = originalColor;
        Debug.Log($"Tile {this.name} reset");
    }
    public void Highlight(Color newColor)
    {
        tileRenderer.material.color = newColor;
    }
}