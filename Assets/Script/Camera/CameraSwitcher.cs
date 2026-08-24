using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    private CinemachineCamera[] cameras;
    public int currentCamera = 0;


    void SetCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].Priority = i == index ? 10 : 0;
    }

    void Start()
    {
        cameras = GetComponentsInChildren<CinemachineCamera>();

        SetCamera(currentCamera);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !GameManager.instance.checking)
        {
            currentCamera++;
            if (currentCamera >= cameras.Length)
                currentCamera = 0;
           
            SetCamera(currentCamera);
        }

        if (Input.GetKeyDown(KeyCode.E) && !GameManager.instance.checking)
        {
          
            currentCamera--;
            if (currentCamera < 0)
                currentCamera = cameras.Length - 1;
            SetCamera(currentCamera);
        }
    }

}
