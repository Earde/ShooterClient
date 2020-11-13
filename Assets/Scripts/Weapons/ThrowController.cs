using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    public Transform camTransform;
    public LocalPlayerController localPlayerController;
    public float throwCooldown = 3.0f;

    private bool readyToThrow = true;

    private IEnumerator ThrowCooldown()
    {
        yield return new WaitForSeconds(throwCooldown);
        readyToThrow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && localPlayerController.IsAlive() && readyToThrow)
        {
            readyToThrow = false;
            ClientSend.PlayerThrowItem(camTransform.forward);
            //audioManager.Throw(); FIX THIS
            StartCoroutine(ThrowCooldown());
        }
    }
}
