using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Node: MonoBehaviour
{
    public Vector2Int cords;

    public Node (Vector2Int cords)
    {
        this.cords = cords;
    }
    public void Initialize(Vector2Int gridcoords)
    {
        cords = gridcoords;
    }
}