using System;
using System.Collections;
using TMPro;
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

    private bool isFrozen = false;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tippingPoint;

    [SerializeField] private Camera mainCamera;

    private Quaternion targetRotation;
    private Vector3 currentAngularVelocity;
    private GridManager gridManager;
    private float xTilt;
    private float zTilt;
    private Vector3 centerOfMass;
    private float totalWeight;

    private void Start()
    {
        currentAngularVelocity = Vector3.zero;
        gridManager = GridManager.Instance;
        tippingPoint.text = $"NEUTRAL";
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            if (GameManager.instance != null && GameManager.instance.gameOver) yield break;

            if (!isFrozen)
            {
                centerOfMass = GetCenterOfMass();
                if (centerOfMass.magnitude > 0.005f)
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

                if (xTilt > 0.1f || xTilt < -0.1f || zTilt > 0.1f || zTilt < -0.1f)
                {
                    Vector3 flatCameraDir = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
                    float dotBetween = Vector3.Dot(flatCameraDir, currentAngularVelocity);
                    if (dotBetween < 0f)
                    {
                        if (dotBetween < 0.5f)
                            tippingPoint.text = "GO FORWARD";
                        else
                            tippingPoint.text = "GO LEFT";
                    }
                    else
                    {
                        if (dotBetween < 0.5f)
                            tippingPoint.text = "GO BACKWARD";
                        else
                            tippingPoint.text = "GO RIGHT";
                    }
                }
                
                if (currentAngularVelocity.magnitude > 1.5f && MusicManager.Instance != null)
                {
                    MusicManager.Instance.PlayBoardTiltingSound(transform.position);
                }
            }

            yield return new WaitForSeconds(secondsPerTick);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, currentAngularVelocity);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up), Vector3.zero);
    }

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
        }

        xCM /= totalWeight;
        yCM /= totalWeight;

        return new Vector3(xCM, 2, yCM);
    }

    public void ApplyPillarManBuff(float weightMultiplier, float accelerationMultiplier)
    {
        if (gridManager != null)
        {
            gridManager.gridWeight *= weightMultiplier;
        }
        tiltAcceleration *= accelerationMultiplier;
    }

    private void Update()
    { }


    public void SetFreeze(bool frozen)
    {
        isFrozen = frozen;
        if (frozen)
        {
            currentAngularVelocity = Vector3.zero;

            StartCoroutine(SmoothLevelRoutine());

            if (MusicManager.Instance != null) MusicManager.Instance.PlayGoldenWindSound(transform.position);
        }
    }

    private IEnumerator SmoothLevelRoutine()
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;
        float duration = 1.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float percentComplete = elapsedTime / duration;
            float smoothPercent = Mathf.SmoothStep(0f, 1f, percentComplete);

            transform.rotation = Quaternion.Slerp(startRotation, Quaternion.identity, smoothPercent);

            xTilt = transform.rotation.x;
            zTilt = transform.rotation.z;

            yield return null; 
        }

        transform.rotation = Quaternion.identity;
        xTilt = 0f;
        zTilt = 0f;
    }
}