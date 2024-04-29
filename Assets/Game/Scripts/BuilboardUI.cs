using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilboardUI : MonoBehaviour
{
    private Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(transform.position + mainCam.rotation * Vector3.forward, mainCam.rotation * Vector3.up);
    }
}
