using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the world space UI face the camera
/// </summary>
public class FaceCamera : MonoBehaviour
{
    Camera myCam;

    void Awake()
    {
        myCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - myCam.transform.position);
    }
}
