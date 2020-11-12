using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InterpolationController : MonoBehaviour
{
    private int stateCount;
    private List<PlayerState> states = new List<PlayerState>();
    private float stateTimer = 0f;
    private bool posEnabled;

    private Quaternion fromRotation = Quaternion.identity;
    private Quaternion toRotation = Quaternion.identity;
    private float rotationTimer = 0f;
    private bool rotEnabled;

    public InterpolationController(bool positionInterpolation, bool rotationInterpolation, int _stateHistoryCount)
    {
        stateCount = _stateHistoryCount;
        posEnabled = positionInterpolation;
        rotEnabled = rotationInterpolation;
        for (int i = 0; i < stateCount; i++)
        {
            states.Add(new PlayerState { position = Vector3.zero, time = 0.0f });
        }
    }

    private void Update()
    {
        if (posEnabled)
        {
            stateTimer += Time.deltaTime;
            transform.position = GetCurrentPosition();
        }
        if (rotEnabled)
        {
            rotationTimer += Time.deltaTime;
            transform.rotation = GetCurrentRotation();
        }
    }

    public void SetNewState(PlayerState newState)
    {
        states.Last().position = GetCurrentPosition();
        states.Last().time = states.Last().time - stateTimer;
        states.Add(newState);
        if (states.Count > stateCount) states.RemoveAt(0);
        stateTimer = 0f;
    }

    public void SetNewRotation(Quaternion quaternion)
    {
        fromRotation = toRotation;
        toRotation = quaternion;
        rotationTimer = 0f;
    }

    public float GetCurrentStateTime()
    {
        float lerpTime = (states.Last().time - states[states.Count - stateCount].time);
        if (lerpTime <= 0.0f) return states.Last().time;
        return states.Last().time - lerpTime + stateTimer;
    }

    public Vector3 GetCurrentPosition()
    {
        float lerpTime = (states.Last().time - states[states.Count - stateCount].time);
        if (lerpTime <= 0.0f) return states.Last().position;
        return Vector3.Lerp(states[states.Count - stateCount].position, states.Last().position, stateTimer / lerpTime);
    }

    public Vector2 GetCurrentMovingDirection()
    {
        Vector2 movingDirection = Vector3.zero;
        if (states.Count < 2) return movingDirection;
        Vector3 currentPos = states[states.Count - 1].position;
        Vector3 prevPos = states[states.Count - 2].position;
        if (currentPos == prevPos) return movingDirection;
        
        float angle = CalculateAngle180(this.transform.forward, (currentPos - prevPos).normalized);
        if (Mathf.Abs(angle) <= 50.0f)
        {
            movingDirection.y = 1.0f;
        } else if (Mathf.Abs(angle) >= 130.0f)
        {
            movingDirection.y = -1.0f;
        }
        if (angle >= 40.0f && angle <= 140.0f)
        {
            movingDirection.x = 1.0f;
        } else if (angle <= -40.0f && angle >= -140.0f)
        {
            movingDirection.x = -1.0f;
        }

        return movingDirection.normalized * 5.0f;
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

    public Quaternion GetCurrentRotation()
    {
        return Quaternion.Lerp(fromRotation, toRotation, rotationTimer / Time.fixedDeltaTime);
    }

    public void SetPositionInterpolation(bool enabled)
    {
        posEnabled = enabled;
    }

    public void SetRotationInterpolation(bool enabled)
    {
        rotEnabled = enabled;
    }
}