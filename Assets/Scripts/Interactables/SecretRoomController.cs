using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SecretRoomController : MonoBehaviour
{
    public Volume volume;
    public BoxCollider boxCollider;
    public Camera cam;

    public GameObject[] secretObjects;

    private void Start()
    {
        Activate(false);
    }

    private void Activate(bool a)
    {
        volume.enabled = a;
        foreach (GameObject go in secretObjects)
        {
            go.SetActive(a);
        }
    }

    private void Update()
    {
        if (!volume.enabled && boxCollider.bounds.Contains(cam.transform.position))
        {
            Activate(true);
        } 
        else if (volume.enabled && !boxCollider.bounds.Contains(cam.transform.position))
        {
            Activate(false);
        }
    }
}
