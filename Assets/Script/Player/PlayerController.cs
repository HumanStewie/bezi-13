using UnityEngine;

public struct CharacterInput
{
    public Vector2 Move;
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform playerMovePoint;
    private enum CharacterState
    {
        Standing,
        Fell,
    }
    private Vector3 requestedMovement;

    public void Initialize()
    {
        playerMovePoint.parent = null;
    }

    public void UpdateInput(CharacterInput input)
    {
        requestedMovement = new Vector3(input.Move.x, 0, input.Move.y);
        requestedMovement = Vector3.ClampMagnitude(requestedMovement, 1.0f);
    }

    public void UpdateBody(float deltaTime)
    {
        playerMovePoint.position += transform.forward;
    }
}
