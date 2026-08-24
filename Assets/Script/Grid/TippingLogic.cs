using System;
using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
    [SerializeField] private CameraSwitcher switcher;
    [SerializeField] private Arrow arrow;

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
                
                
                
                if (currentAngularVelocity.magnitude > 1.5f && MusicManager.Instance != null)
                {
                    MusicManager.Instance.PlayBoardTiltingSound(transform.position);
                }
            }

            yield return new WaitForSeconds(secondsPerTick);
        }
    }

    private void Update()
    {
        if (xTilt > 0.02f || xTilt < -0.02f || zTilt > 0.02f || zTilt < -0.02f)
        {
            arrow.gameObject.SetActive(true);
            Vector3 flatCameraDir = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            Vector3 flatNormal = Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized;
            Vector3 directionToPointTo = Vector3.ProjectOnPlane(-flatNormal, transform.up).normalized + Vector3.up;
            arrow.transform.LookAt(directionToPointTo + GameManager.instance.playerEntity.transform.position);
            float dotBetween = Vector3.Dot(flatCameraDir, flatNormal);
            float angleBetween = Vector3.Angle(flatCameraDir, flatNormal);

            int temp = 0;
            bool corner = false;
    
            if (dotBetween < 0f)
            {
                if (angleBetween <= 135 && flatNormal.z < 0)
                    temp = 0; //"GO FORWARD";
                else
                    temp = 3; //"GO RIGHT"; 
            }
            else
            {
                if (angleBetween < 45 && flatNormal.x > 0)
                    temp = 1; //"GO LEFT";
                else
                    temp = 2; //"GO BACKWARD";
            }

            temp = (temp + switcher.currentCamera)%4;
    
            if (temp == 0) tippingPoint.text = "GO FORWARD";
            else if (temp == 1) tippingPoint.text = "GO LEFT";
            else if (temp == 2) tippingPoint.text = "GO BACKWARD";
            else if (temp == 3) tippingPoint.text = "GO RIGHT";

            // corners
            temp = 0;

            if (flatNormal.z < 0 && flatNormal.x < 0)
            {
                temp = 0; //"GO FORWARD & RIGHT";
                corner = true;
            }
            if (flatNormal.z < 0 && flatNormal.x > 0)
            {
                temp = 1; //"GO FORWARD & LEFT";
                corner = true;
            }

            if (flatNormal.z > 0 && flatNormal.x > 0)
            {
                temp = 2; //"GO BACKWARD & LEFT"; 
                corner = true;
            }

            if (flatNormal.z > 0 && flatNormal.x < 0)
            {
                temp = 3; //"GO BACKWARD & RIGHT";
                corner = true;
            }

            if (corner)
            {
                temp = (temp + switcher.currentCamera)%4;
                if (temp == 0) tippingPoint.text = "GO FORWARD & RIGHT";
                else if (temp == 1) tippingPoint.text = "GO FORWARD & LEFT";
                else if (temp == 2) tippingPoint.text = "GO BACKWARD & LEFT";
                else if (temp == 3) tippingPoint.text = "GO BACKWARD & RIGHT";
            }
    
        }
        else
        {
            arrow.TurnOff();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized, Vector3.zero);
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