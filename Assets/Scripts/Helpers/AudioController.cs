using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioSources
/// </summary>
public class AudioController : MonoBehaviour
{
    public AudioSource damageAudio, dieAudio;

    public void TakeDamage()
    {
        damageAudio.Play();
    }

    public void Die()
    {
        dieAudio.Play();
    }
}
