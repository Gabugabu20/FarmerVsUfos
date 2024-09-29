using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBGMusic : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        audioSource.Play();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }
}
