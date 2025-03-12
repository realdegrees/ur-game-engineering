using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Background Music")]
    public AudioSource backgroundMusic;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float backgroundMusicVolume = 1f;
    [Range(0f, 1f)] public float duckingFactor = 0.3f;

    private static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("AudioManager already exists in the scene");
        }
        instance = this;
    }

    void Start()
    {
        backgroundMusic.volume = backgroundMusicVolume;
        PlayMusic();
    }

    public void PlayMusic(bool loop = true)
    {
        if(backgroundMusic.clip == null)
        {
            Debug.LogWarning("No audio clip is assigned to backgroundMusic.");
            return;
        }
        backgroundMusic.loop = loop;
        backgroundMusic.Play();
    }

    public void SetMusicVolume(float volume)
    {
        backgroundMusicVolume = volume;
        backgroundMusic.volume = volume;
    }
}
