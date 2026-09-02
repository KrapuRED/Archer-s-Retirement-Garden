using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] protected Transform objectTransform;
    private Transform _targetCamera;

    private void LateUpdate()
    {
        if (_targetCamera == null) 
            _targetCamera = GetCameraTransform();
        
        LookAtCamera(_targetCamera);
    }

    private Transform GetCameraTransform()
    {
        if (_targetCamera != null && _targetCamera.gameObject.activeInHierarchy) return _targetCamera;

        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy)
            {
                return cam.transform;
            }
        }
        
        return null;
    }
    
    private void LookAtCamera(Transform cameraTransform)
    {
        if (objectTransform == null || cameraTransform == null) return;
        Vector3 targetPosition = cameraTransform.position;
        targetPosition.y = objectTransform.position.y;
        
        objectTransform.LookAt(targetPosition);
    }
}
