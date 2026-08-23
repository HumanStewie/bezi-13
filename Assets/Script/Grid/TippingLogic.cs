using System;
using System.Collections;
using TMPro;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.UI;

public class TippingLogic : MonoBehaviour
{
    [Header("Physics Settings")] 
    [SerializeField] private float secondsPerTick = 3f;

    [SerializeField] private float fixedStep = 0.33f;
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
    private Vector3 centerOfMass;
    private float totalEntityInertia;
    private float totalWeight;
    private void Start()
    {
        currentAngularVelocity = Vector3.zero;
        gridManager = GridManager.Instance;
        tippingPoint.text = $"X-Axis Tilt: {xTilt}\nZ-Axis Tilt: {zTilt}";
        StartCoroutine(Rotate());
    }
    
    /// <summary>
    /// IEnumerator but not necessary. Get Center of Mass, get torque from it, which is also acceleration, stripped out of magnitude
    /// So we apply it again then add it together.
    /// Then we can do some clamping and damping. Lastly just apply the rotation.
    /// TargetRotation is actually angular velocity, made into quaternion with AngleAxis(). So its acting like a multiplier more.
    ///
    /// Basic unbalanced 2D pendulum
    /// </summary>
    /// <returns></returns>
    IEnumerator Rotate()
    {
        while (true)
        {
            centerOfMass = GetCenterOfMass();
            if (centerOfMass.magnitude > 0.005)
            {
                Vector3 tiltAxis = Vector3.Cross(Vector3.up, centerOfMass);
                
                Vector3 angularAcceleration = tiltAxis * tiltAcceleration;
                currentAngularVelocity += angularAcceleration;
            }
            else
            {
                currentAngularVelocity = Vector3.zero;
            }

            currentAngularVelocity *= damping;
            currentAngularVelocity = Vector3.ClampMagnitude(currentAngularVelocity, maxAngularVelocity);
            targetRotation = Quaternion.AngleAxis(currentAngularVelocity.magnitude * fixedStep, currentAngularVelocity.normalized);
            
            transform.rotation = targetRotation * transform.rotation;
            xTilt = transform.rotation.x;
            zTilt = transform.rotation.z;
            yield return new WaitForSeconds(secondsPerTick);
        }
    }


    /// <summary>
    /// Calculate weight of every entities on the board then output current center of mass
    /// </summary>
    /// <returns></returns>
    private Vector3 GetCenterOfMass()
    {
        totalWeight = gridManager.gridWeight;
        float xCM = 0f;
        float yCM = 0f;
        
        foreach (Entity entity in FindObjectsByType<Entity>(FindObjectsSortMode.None))
        {   
            totalWeight += entity.weight;
            xCM += entity.coords.x * entity.weight;
            yCM += entity.coords.y * entity.weight;
            totalEntityInertia += entity.weight * Mathf.Pow(gridManager.GetDistance(entity.coords, Vector2Int.zero), 2);
        }
        
        xCM /= totalWeight;
        yCM /= totalWeight;
        
        return new Vector3(xCM, 2, yCM);
    }

    private void Update()
    {
        tippingPoint.text = $"WS Tilt: {xTilt}\nAD Tilt: {zTilt}";
    }
}
