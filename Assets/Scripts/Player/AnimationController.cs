using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public InterpolationController interpolationController;

    private void FixedUpdate()
    {
        Vector2 moveDir = interpolationController.GetCurrentMovingDirection();
        animator.SetFloat("xVelocity", moveDir.x);
        animator.SetFloat("zVelocity", moveDir.y);
    }
}
