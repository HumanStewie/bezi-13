using System;
using System.Collections;
using TMPro;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.UI;

public class TippingLogic : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float ticksPerSecond = 3f;
    [SerializeField] private float tiltAcceleration = 45f;
    [SerializeField] private float maxAngularVelocity = 30f;
    [SerializeField] private float damping = 0.95f;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tippingPoint;
    private Quaternion targetRotation;
    private Vector3 currentAngularVelocity;
    private GridManager gridManager;
    private float xTilt;
    private float zTilt;

    private void Start()
    {
        currentAngularVelocity = Vector3.zero;
        gridManager = GridManager.Instance;
        tippingPoint.text = $"X-Axis Tilt: {xTilt}\nZ-Axis Tilt: {zTilt}";
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
                Vector3 angularAcceleration = tiltAxis * (centerOfMass.magnitude * tiltAcceleration);
                currentAngularVelocity += angularAcceleration;
            }
            else
            {
                currentAngularVelocity = Vector3.zero;
            }

            currentAngularVelocity *= damping;
            currentAngularVelocity = Vector3.ClampMagnitude(currentAngularVelocity, maxAngularVelocity);
            targetRotation = Quaternion.AngleAxis(currentAngularVelocity.magnitude * Time.deltaTime, currentAngularVelocity.normalized);
            transform.rotation = targetRotation * transform.rotation;
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

}
