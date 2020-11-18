using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public LocalPlayerController player;
    public float clampAngle = 85f;

    private float sensitivity = 2.0f;
    private float verticalRotation;
    private float horizontalRotation;

    public void SetMouseSensitivity(float sens)
    {
        if (sens > 0.0f) { sensitivity = sens; }
    }

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
        ToggleCursorMode(true);
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked) { Look(); }
    }

    private void Look()
    {
        float mouseVertical = -Input.GetAxisRaw("Mouse Y");
        float mouseHorizontal = Input.GetAxisRaw("Mouse X");

        verticalRotation += mouseVertical * sensitivity;
        horizontalRotation += mouseHorizontal * sensitivity;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }

    public void ToggleCursorMode(bool lockMouse)
    {
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
