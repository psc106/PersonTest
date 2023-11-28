using Cinemachine;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : ValidatedMonoBehaviour
{
    [Header("Refrences")]
    [SerializeField, Anywhere] InputReader input;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookCam;

    [Header("Settings")]
    [SerializeField, Range(0.5f, 3f)] float SpeedMulitiplier = 1f;

    bool isRMBPressed;
    bool cameraMovementLock;


    private void OnEnable()
    {
        input.Look += OnLook;
        input.EnableMouseControlCamera += OnEnableMouseControlCamera;
        input.DisableMouseControlCamera += OnDisableMouseControlCamera;
    }
    private void OnDisable()
    {
        input.Look -= OnLook;
        input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
        input.DisableMouseControlCamera -= OnDisableMouseControlCamera;

    }

    void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
    {
        if (cameraMovementLock) return;
        if (isDeviceMouse && !isRMBPressed) return;

        float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

        freeLookCam.m_XAxis.m_InputAxisValue = cameraMovement.x * SpeedMulitiplier * deviceMultiplier;
        freeLookCam.m_YAxis.m_InputAxisValue = cameraMovement.y * SpeedMulitiplier * deviceMultiplier;


    }

    void OnEnableMouseControlCamera()
    {
        Debug.Log("우클릭중");
        isRMBPressed = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(DisableMouseForFrame());
    }
    void OnDisableMouseControlCamera()
    {
        Debug.Log("우클릭안함");
        isRMBPressed = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        freeLookCam.m_XAxis.m_InputAxisValue = 0f;
        freeLookCam.m_YAxis.m_InputAxisValue = 0f;
    }

    IEnumerator DisableMouseForFrame()
    {
        cameraMovementLock = true;
        yield return new WaitForEndOfFrame();
        cameraMovementLock = false;


    }

}
