using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource[] movementAudios;

    public CharacterController characterController;

    private int curMovement = 0;

    /// <summary>
    /// Play audio when moving
    /// </summary>
    /// <param name="movementDirection"></param>
    public void Move(Vector3 movementDirection)
    {
        if ((characterController == null || characterController.isGrounded) && !movementAudios[curMovement].isPlaying && (movementDirection.x != 0.0f || movementDirection.z != 0.0f))
        {
            curMovement++;
            if (curMovement >= movementAudios.Length) curMovement = 0;
            movementAudios[curMovement].Play();
        }
    }
}
