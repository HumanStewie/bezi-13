using UnityEngine;
using Unity.Cinemachine; // Required for Cinemachine 3.x

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    private CinemachineBasicMultiChannelPerlin perlinNoise;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private void Awake()
    {

        perlinNoise = GetComponent<CinemachineBasicMultiChannelPerlin>();

        perlinNoise.AmplitudeGain = 0f;
    }
    public void ShakeCamera(float shakeIntensity, float duration)
    {
            perlinNoise.AmplitudeGain = shakeIntensity;
            startingIntensity = shakeIntensity;
            shakeTimer = duration;
            shakeTimerTotal = duration;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (perlinNoise != null)
            {
                perlinNoise.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }
}