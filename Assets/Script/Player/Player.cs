using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    
    public Vector2Int currentPosition = Vector2Int.zero;

    [SerializeField] private float angleTransTime = 15f;

    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }
    void Update()
    {
        currentPosition = GetComponent<Entity>().coords;
        StartCoroutine(MovementDelay());
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * angleTransTime);

    }
    void Movement()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (GridManager.Instance.CheckTileExistence(currentPosition + new Vector2Int(0, 1)))
            {
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), currentPosition + new Vector2Int(0, 1));
                targetRotation = Quaternion.Euler(-90, -90f, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (GridManager.Instance.CheckTileExistence(currentPosition + new Vector2Int(0, -1)))
            {
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), currentPosition + new Vector2Int(0, -1));
                targetRotation = Quaternion.Euler(-90, 90, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (GridManager.Instance.CheckTileExistence(currentPosition + new Vector2Int(1, 0)))
            {
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), currentPosition + new Vector2Int(1, 0));
                targetRotation = Quaternion.Euler(-90, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (GridManager.Instance.CheckTileExistence(currentPosition + new Vector2Int(-1, 0)))
            {
                GridManager.Instance.MoveEntity(this.GetComponent<Entity>(), currentPosition + new Vector2Int(-1, 0));
                targetRotation = Quaternion.Euler(-90, 180, 0);
            }
        }
    }
    IEnumerator MovementDelay()
    {
        Movement();
        yield return new WaitForSeconds(0.07f);
    }
}
