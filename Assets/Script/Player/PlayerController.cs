using UnityEngine;

public struct CharacterInput
{
    public Vector2 Move;
}



public class PlayerController : MonoBehaviour
{
    
    private enum CharacterState
    {
        Standing,
        Fell,
    }
    public void Initialize()
    {
        
    }
}
