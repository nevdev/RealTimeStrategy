using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

        
    // makes the Health Bar continously facing the camera
    private void LateUpdate()
    {// makes an UI facing the camera when the object of the UI can move around and spins
        transform.LookAt(
            transform.position + mainCameraTransform.rotation * Vector3.forward,
            mainCameraTransform.rotation * Vector3.up
            );
    }
}
