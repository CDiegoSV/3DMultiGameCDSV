using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookToCamera : MonoBehaviour
{
    private Transform pointToLookAt;

    private void Awake()
    {
        pointToLookAt = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(pointToLookAt);
    }
}
