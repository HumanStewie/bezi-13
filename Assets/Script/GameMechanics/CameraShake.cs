using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private List<CinemachineBasicMultiChannelPerlin> allNoises = new List<CinemachineBasicMultiChannelPerlin>();

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        CinemachineCamera[] allCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);

        foreach (var cam in allCams)
        {
            var noise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

            if (noise != null)
            {
                noise.AmplitudeGain = 0f; 
                allNoises.Add(noise);   
            }
        }
    }

    public void ShakeCamera(float shakeIntensity, float duration)
    {
        startingIntensity = shakeIntensity;
        shakeTimer = duration;
        shakeTimerTotal = duration;

        foreach (var noise in allNoises)
        {
            if (noise != null) noise.AmplitudeGain = shakeIntensity;
        }
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            float currentIntensity = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));

            foreach (var noise in allNoises)
            {
                if (noise != null) noise.AmplitudeGain = currentIntensity;
            }
        }
    }
}