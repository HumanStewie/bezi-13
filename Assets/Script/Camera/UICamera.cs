using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation * Quaternion.Euler(90f, 0, 0);
    }

    void Update()
    {
        transform.position = target.position;
        transform.rotation =  target.rotation * Quaternion.Euler(90f, 0, 0);
    }
}
