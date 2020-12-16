using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitController : MonoBehaviour
{
    public float sitRadius = 10.0f;
    public int layerId = 0;
    public Camera camera;

    private int layerMask;

    private void Start()
    {
        layerMask = 1 << layerId;
    }

    void Update()
    {
        if (!camera.enabled) return;

        // Sit
        Collider[] hits = Physics.OverlapSphere(transform.position, sitRadius, layerMask);
        if (hits.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Sittable sit = hits[0].gameObject.GetComponent<Sittable>();
                sit.Sit(camera);
            }
        }
    }
}
