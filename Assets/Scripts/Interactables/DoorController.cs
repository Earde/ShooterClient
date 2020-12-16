using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float rangeRadius = 20.0f;
    public float upDistance = 14.0f;
    public bool isOpen = false;
    public float speed = 5.0f;

    private bool isMoving = false;
    private Vector3 basePosition, upPosition, targetPosition;

    private void Start()
    {
        basePosition = transform.position;
        if (isOpen)
        {
            upPosition = basePosition;
            basePosition = new Vector3(basePosition.x, basePosition.y - upDistance, basePosition.z);
        } else
        {
            upPosition = new Vector3(basePosition.x, basePosition.y + upDistance, basePosition.z);
        }
    }

    void Update()
    {
        //Open/close door
        if (!isMoving && Input.GetKeyDown(KeyCode.E))
        {
            isMoving = true;
            isOpen = !isOpen;
            if (isOpen) targetPosition = upPosition;
            else targetPosition = basePosition;
        }
        //Move door
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isMoving = false;
            }
        }
    }
}
