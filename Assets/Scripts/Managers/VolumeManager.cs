using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Change Volume profile
/// </summary>
public class VolumeManager : MonoBehaviour
{
    public Volume volume;
    public VolumeProfile[] profiles = new VolumeProfile[3];

    public void SetLow()
    {
        volume.profile = profiles[0];
    }

    public void SetMedium()
    {
        volume.profile = profiles[1];
    }

    public void SetUltra()
    {
        volume.profile = profiles[2];
    }
}
