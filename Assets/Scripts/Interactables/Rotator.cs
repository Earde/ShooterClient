using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed = 5.0f;

    void Update()
    {
        transform.Rotate(Vector3.up, 45f * Time.deltaTime * rotateSpeed);
    }
}
