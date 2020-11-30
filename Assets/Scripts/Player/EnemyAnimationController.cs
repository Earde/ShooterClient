using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    public Animator animator;
    public InterpolationController interpolationController;

    public void Move(Vector3 movementDirection, Vector3 camForward)
    {
        Vector2 moveDir = GetCurrentMovingDirection(movementDirection, camForward);
        animator.SetFloat("xVelocity", moveDir.x);
        animator.SetFloat("zVelocity", moveDir.y);
    }

    public Vector2 GetCurrentMovingDirection(Vector3 movementDirection, Vector3 camForward)
    {
        Vector2 movDir = Vector3.zero;

        if (movementDirection == Vector3.zero || movementDirection.magnitude < 0.00001f) return movDir;

        float angle = CalculateAngle180(camForward, movementDirection.normalized);
        if (Mathf.Abs(angle) <= 50.0f)
        {
            movDir.y = 1.0f;
        }
        else if (Mathf.Abs(angle) >= 130.0f)
        {
            movDir.y = -1.0f;
        }
        if (angle >= 40.0f && angle <= 140.0f)
        {
            movDir.x = 1.0f;
        }
        else if (angle <= -40.0f && angle >= -140.0f)
        {
            movDir.x = -1.0f;
        }

        return movDir.normalized * 5.0f;
    }

    /// <summary>
    /// Get euler angle between two direction (-180 to 180)
    /// </summary>
    /// <param name="fromDir"></param>
    /// <param name="toDir"></param>
    /// <returns></returns>
    private static float CalculateAngle180(Vector3 fromDir, Vector3 toDir)
    {
        float angle = Quaternion.FromToRotation(fromDir, toDir).eulerAngles.y;
        if (angle > 180) { return angle - 360f; }
        return angle;
    }
}
