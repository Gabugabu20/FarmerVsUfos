using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{
    private AudioSource mooAudioSource;

    void Awake()
    {
        mooAudioSource = GetComponent<AudioSource>();
        mooAudioSource.Play();
        mooAudioSource.Pause();
    }

    public void StartMooSound()
    {
        mooAudioSource.UnPause();
    }

    public void StopMooSound()
    {
        mooAudioSource.Pause();
    }
}
