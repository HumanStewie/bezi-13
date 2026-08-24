using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
    public Vector2Int cords;

    private Mesh startSprite;

    private Renderer tileRenderer;
    private MeshFilter filter;
    private Renderer iconRenderer;

    private Color initColor;
    public void Start()
    {
        filter = GetComponentInChildren<MeshFilter>();
        startSprite = filter.mesh;
        tileRenderer = GetComponentInChildren<MeshRenderer>();
        iconRenderer = GetComponentInChildren<SpriteRenderer>();
        initColor = iconRenderer.material.color;
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
        Debug.Log($"Tile {this.name} reset");
    }

    public void HighlightNode(Color color)
    {
        iconRenderer.material.color = color;
    }
    public void Highlight(Color lightColor, Color darkColor)
    {
        // Color Light
        tileRenderer.materials[2].SetColor("Color_9bbf2ad544ff418eb92f2bc07389403b", lightColor);
        
        // Color Dark
        tileRenderer.materials[2].SetColor("Color_f45cb926e67e4d7887dae7cc6dbcffb2", darkColor);
    }
}