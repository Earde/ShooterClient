using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InterpolationController : MonoBehaviour
{
    private List<PlayerState> states = new List<PlayerState>();

    private bool posEnabled;
    private bool rotEnabled;

    private float saveTime = 1.0f;

    private float delay = 0.1f;

    private bool isLocalPlayer;

    public InterpolationController(bool _isLocalPlayer, bool positionInterpolation, bool rotationInterpolation)
    {
        isLocalPlayer = _isLocalPlayer;
        if (isLocalPlayer) delay = 0.0f;
        posEnabled = positionInterpolation;
        rotEnabled = rotationInterpolation;
        for (int i = 0; i < 10; i++)
        {
            states.Add(new PlayerState { _position = Vector3.zero, _rotation = Quaternion.identity, _yVelocity = 0.0f, _time = 0.0f });
        }
    }

    protected virtual void Start() { }

    protected virtual void Update()
    {
        if (posEnabled)
        {
            transform.position = GetCurrentPosition(Time.time - delay);
        }
        if (rotEnabled)
        {
            transform.rotation = GetCurrentRotation(Time.time - delay);
        }
    }

    public void AddPlayerState(PlayerState newState)
    {
        states.Add(newState);
        states.RemoveAll(x => x._time < Time.time - saveTime);
    }

    /// <summary>
    /// Get position on time x
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private Vector3 GetCurrentPosition(float time)
    {
        PlayerState fromState = states.Where(s => s._time < time).OrderByDescending(x => x._time).FirstOrDefault();
        if (fromState == default || fromState == null) return transform.position;
        PlayerState toState = states.Where(s => s._time >= time).OrderBy(x => x._time).FirstOrDefault();
        if (toState == default || toState == null) return fromState._position;
        float lerp = (time - fromState._time) / (toState._time - fromState._time);
        return Vector3.Lerp(fromState._position, toState._position, lerp);
    }

    /// <summary>
    /// Get rotation on time x
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private Quaternion GetCurrentRotation(float time)
    {
        PlayerState fromState = states.Where(s => s._time < time).OrderByDescending(x => x._time).FirstOrDefault();
        if (fromState == default || fromState == null) return transform.rotation;
        PlayerState toState = states.Where(s => s._time >= time).OrderBy(x => x._time).FirstOrDefault();
        if (toState == default || toState == null) return fromState._rotation;
        float lerp = (time - fromState._time) / (toState._time - fromState._time);
        return Quaternion.Lerp(fromState._rotation, toState._rotation, lerp);
    }
}