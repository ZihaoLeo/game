using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioSource audioSource;
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public AudioMixer audioMixer; // Drag the AudioMixer into this field
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
