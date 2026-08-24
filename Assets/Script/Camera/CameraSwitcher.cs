using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    private CinemachineCamera[] cameras;
    private int currentCamera = 0;

    void SetCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].Priority = i == index ? 10 : 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameras = GetComponentsInChildren<CinemachineCamera>();
        SetCamera(currentCamera);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentCamera++;
            if (currentCamera >= cameras.Length)
                currentCamera = 0;
           
            SetCamera(currentCamera);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
          
            currentCamera--;
            if (currentCamera < 0)
                currentCamera = cameras.Length - 1;
            SetCamera(currentCamera);
        }
    }
}
