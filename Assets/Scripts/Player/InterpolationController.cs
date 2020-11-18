using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InterpolationController : MonoBehaviour
{
    private List<PlayerState> states = new List<PlayerState>();

    private bool posEnabled;
    private bool rotEnabled;

    private float delay = 0.1f;

    private bool isLocalPlayer;

    private Vector3 prevPos = Vector3.zero;
    private Vector3 movingDirection = Vector3.zero;

    public InterpolationController(bool _isLocalPlayer, bool positionInterpolation, bool rotationInterpolation)
    {
        isLocalPlayer = _isLocalPlayer;
        if (isLocalPlayer) delay = 0.0f;
        posEnabled = positionInterpolation;
        rotEnabled = rotationInterpolation;
        for (int i = 0; i < 10; i++)
        {
            states.Add(new PlayerState { position = Vector3.zero, time = 0.0f });
        }
    }

    private void Update()
    {
        if (posEnabled)
        {
            transform.position = GetCurrentPosition(Time.time - delay);
        }
        if (rotEnabled)
        {
            transform.rotation = GetCurrentRotation(Time.time - delay);
        }
        movingDirection = transform.position - prevPos;
        prevPos = transform.position;
    }

    public void AddPlayerState(PlayerState newState)
    {
        states.Add(newState);
        states.RemoveAll(x => x.time < Time.time - 1f);
    }

    /// <summary>
    /// Get position on time x
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private Vector3 GetCurrentPosition(float time)
    {
        PlayerState fromState = states.Where(s => s.time < time).OrderByDescending(x => x.time).FirstOrDefault();
        if (fromState == default || fromState == null) return transform.position;
        PlayerState toState = states.Where(s => s.time >= time).OrderBy(x => x.time).FirstOrDefault();
        if (toState == default || toState == null) return fromState.position;
        float lerp = (time - fromState.time) / (toState.time - fromState.time);
        return Vector3.Lerp(fromState.position, toState.position, lerp);
    }

    /// <summary>
    /// Get rotation on time x
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private Quaternion GetCurrentRotation(float time)
    {
        PlayerState fromState = states.Where(s => s.time < time).OrderByDescending(x => x.time).FirstOrDefault();
        if (fromState == default || fromState == null) return transform.rotation;
        PlayerState toState = states.Where(s => s.time >= time).OrderBy(x => x.time).FirstOrDefault();
        if (toState == default || toState == null) return fromState.rotation;
        float lerp = (time - fromState.time) / (toState.time - fromState.time);
        return Quaternion.Lerp(fromState.rotation, toState.rotation, lerp);
    }

    public Vector2 GetCurrentMovingDirection()
    {
        Vector2 movDir = Vector3.zero;

        if (movingDirection == Vector3.zero || movingDirection.magnitude < 0.00001f) return movDir;
        
        float angle = CalculateAngle180(this.transform.forward, movingDirection.normalized);
        if (Mathf.Abs(angle) <= 50.0f)
        {
            movDir.y = 1.0f;
        } else if (Mathf.Abs(angle) >= 130.0f)
        {
            movDir.y = -1.0f;
        }
        if (angle >= 40.0f && angle <= 140.0f)
        {
            movDir.x = 1.0f;
        } else if (angle <= -40.0f && angle >= -140.0f)
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