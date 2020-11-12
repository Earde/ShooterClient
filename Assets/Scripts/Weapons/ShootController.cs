using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public Transform camTransform;
    public PlayerController playerManager;
    public float throwCooldown = 3.0f;

    private bool readyToThrow = true;

    private IEnumerator ThrowCooldown()
    {
        yield return new WaitForSeconds(throwCooldown);
        readyToThrow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && playerManager.IsAlive() && readyToThrow)
        {
            readyToThrow = false;
            ClientSend.PlayerThrowItem(camTransform.forward);
            //audioManager.Throw(); FIX THIS
            StartCoroutine(ThrowCooldown());
        }
    }
}
