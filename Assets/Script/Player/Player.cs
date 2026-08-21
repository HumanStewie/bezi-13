using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    
    private PlayerActionInput playerInput;
    void Start()
    {
        playerInput  = new PlayerActionInput();
        playerInput.Enable();
        playerController.Initialize();
    }

    void Update()
    {
        var input = playerInput.Player;
        var deltaTime = Time.deltaTime;
        var characterInput = new CharacterInput
        {
            Move = input.Move.ReadValue<Vector2>(),
        };
        playerController.UpdateInput(characterInput);
        playerController.UpdateBody(deltaTime);
    }
}
