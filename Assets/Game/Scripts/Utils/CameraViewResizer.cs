using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewResizer : MonoBehaviour
{
    public float referenceWidth = 1920f;
    public float referenceHeight = 1080f;
    public float referenceOrthographicSize = 4.5f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float targetAspect = referenceWidth / referenceHeight;
        float currentAspect = (float)Screen.width / (float)Screen.height;

        if (currentAspect >= targetAspect)
        {
            cam.orthographicSize = referenceOrthographicSize;
        }
        else
        {
            float differenceInSize = targetAspect / currentAspect;
            cam.orthographicSize = referenceOrthographicSize * differenceInSize;
        }
    }
}
