using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float timePerBlink;

    private bool visible = true;
    private void Start()
    {
    }

    public void TurnOff()
    {
        visible = false;
        gameObject.SetActive(false);
    }
}
