using System;
using System.Collections;
using Unity.IntegerTime;
using UnityEngine;

public class TippingLogic : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float pivotFriction = 0.5f;
    [SerializeField] private float platformMomentOfInertia = 50f;
    [SerializeField] private float slerpResponse = 15f;
    [SerializeField] private float ticksPerSecond = 3f;
    private Vector3 targetRotation;
    private Vector3 angularVelocity;
    private GridManager gridManager;

    private void Start()
    {
        gridManager = GridManager.Instance;
        
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            Vector3 centerOfMass = GetCenterOfMass();
            if (centerOfMass.magnitude > 0.005)
            {
                Vector3 tiltAxis = Vector3.Cross(Vector3.up, centerOfMass).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(centerOfMass.magnitude * 10f, tiltAxis), 1.0f - Mathf.Exp(-slerpResponse * Time.deltaTime));
            }
            else
                transform.rotation = Quaternion.identity;
            yield return new WaitForSeconds(ticksPerSecond);
        }
    }
    
    /// <summary>
    /// Calculate weight of every entities on the board then output current center of mass
    /// </summary>
    /// <returns></returns>
    private Vector3 GetCenterOfMass()
    {
        float totalWeight = gridManager.gridWeight;
        float xCM = 0f;
        float yCM = 0f;
        
        foreach (Entity entity in GridManager.Instance.entities.Values)
        {
            totalWeight += entity.weight;
            xCM += entity.coords.x * entity.weight;
            yCM += entity.coords.y * entity.weight;
        }
        
        xCM /= totalWeight;
        yCM /= totalWeight;

        return new Vector3(xCM, 0, yCM);
    }

    private Vector3 GetTotalTorque()
    {
        Vector3 torque = new Vector3();
        
        
        foreach (var entity in GridManager.Instance.entities.Values)
        {
            torque += Vector3.Cross(gridManager.CoordToWorldPos(entity.coords), gravity * entity.weight * Vector3.up);
        }

        torque.y = 0;
        return torque;
    }
}
