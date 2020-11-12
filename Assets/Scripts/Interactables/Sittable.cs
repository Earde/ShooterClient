using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sittable : MonoBehaviour
{
    public float cameraSensitivy;
    public Camera camera;

    public Vector2 maxAngle;

    private float verticalRotation;
    private float horizontalRotation;

    private Camera tempCam;

    private void Start()
    {
        camera.enabled = false;
    }

    private void Update()
    {
        if (camera.enabled)
        {
            float mouseVertical = -Input.GetAxisRaw("Mouse Y");
            float mouseHorizontal = Input.GetAxisRaw("Mouse X");

            verticalRotation += mouseVertical * cameraSensitivy;
            horizontalRotation += mouseHorizontal * cameraSensitivy;

            verticalRotation = Mathf.Clamp(verticalRotation, -maxAngle.y, maxAngle.y);
            horizontalRotation = Mathf.Clamp(verticalRotation, -maxAngle.x, maxAngle.x);

            transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        }
    }

    public void Sit(Camera oldCam)
    {
        tempCam = oldCam;
        tempCam.enabled = false;
        camera.enabled = true;
    }

    public void Standup()
    {
        camera.enabled = false;
        tempCam.enabled = true;
    }
}
