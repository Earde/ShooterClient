using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Vector3 rotateAround = new Vector3(0, 0, 0);

    public float distance = 250.0f;

    public float speed = 20.0f;

    private void Update()
    {
        transform.RotateAround(rotateAround, Vector3.up, speed * Time.deltaTime);
    }
}
