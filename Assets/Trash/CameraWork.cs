using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

public class CameraWork : MonoBehaviour
{

    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera driftCam;
    public GameObject follower;
    public GameObject aimee;
    public bool isActive = false;

    public void camDriift(InputAction.CallbackContext context)
    {
        if (isActive)
        {
            GetComponent<PlayerInput>();
            bool btn = context.ReadValueAsButton();
            if (btn)
                driftCam.Priority = 11;
            else
                driftCam.Priority = 9;
        }
    }
}
