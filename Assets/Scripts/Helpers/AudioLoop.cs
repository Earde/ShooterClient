using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoop : MonoBehaviour
{
    public GameObject audioObject;

    private List<AudioSource> audioSources = new List<AudioSource>();
    private int currentSong = 0;

    void Start()
    {
        foreach (AudioSource audio in audioObject.GetComponents<AudioSource>())
        {
            audioSources.Add(audio);
        }
        currentSong = Random.Range(0, audioSources.Count);
        StartCoroutine(PlaySong());
    }

    private void NextMenuSong()
    {
        currentSong++;
        if (currentSong >= audioSources.Count) currentSong = 0;
        audioSources[currentSong].Play();
    }

    private IEnumerator PlaySong()
    {
        yield return new WaitForSeconds(2.5f);
        if (!audioSources[currentSong].isPlaying) NextMenuSong();
        StartCoroutine(PlaySong());
    }
}
