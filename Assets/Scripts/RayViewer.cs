using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayViewer : MonoBehaviour
{

    public Camera playerCam;

    void Update()
    {
        Vector3 lineOrigin = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(lineOrigin, playerCam.transform.TransformDirection(0, 0, 1) * float.MaxValue, Color.green);
    }
}
