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
    }

    void Update()
    {
        
    }
}
