using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private List<CinemachineVirtualCamera> cameraList; 
    private int currentCameraIndex = 0;

    private void Start()
    {
        DisableAllCameras();

        if (cameraList.Count > 0)
        {
            EnableCamera(currentCameraIndex);
        }
    }

    public void NextCamera()
    {
        if (cameraList.Count == 0) return;

        DisableCamera(currentCameraIndex);
        currentCameraIndex = (currentCameraIndex + 1) % cameraList.Count;
        EnableCamera(currentCameraIndex);
    }

    public void PreviousCamera()
    {
        if (cameraList.Count == 0) return;

        DisableCamera(currentCameraIndex);
        currentCameraIndex = (currentCameraIndex - 1 + cameraList.Count) % cameraList.Count;
        EnableCamera(currentCameraIndex);
    }

    private void DisableAllCameras()
    {
        foreach (var cam in cameraList)
        {
            cam.Priority = 0; 
        }
    }

    private void EnableCamera(int index)
    {
        cameraList[index].Priority = 10;
    }

    private void DisableCamera(int index)
    {
        cameraList[index].Priority = 0;
    }
}
