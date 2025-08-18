using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class CamMove: MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCam = null;


    void Start()
    {
        virtualCam.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            virtualCam.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            virtualCam.enabled = false;
        }
    }

    void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}

