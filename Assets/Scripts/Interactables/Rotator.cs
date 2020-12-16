using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed = 5.0f;

    /// <summary>
    /// Rotate object at Y-axis
    /// </summary>
    void Update()
    {
        transform.Rotate(Vector3.up, 45f * Time.deltaTime * rotateSpeed);
    }
}
