using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{
    private AudioSource mooAudioSource;

    public bool IsBeingTargeted { get; set; } = false;

    void Awake()
    {
        mooAudioSource = GetComponent<AudioSource>();
        mooAudioSource.Play();
        mooAudioSource.Pause();
    }
    void OnEnable()
    {
        CowManager.Instance.RegisterCow(this);
    }

    void OnDestroy()
    {
        CowManager.Instance.UnregisterCow(this);
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
