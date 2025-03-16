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

    public static AudioManager instance { get; private set; }

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
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = backgroundMusicVolume;
            PlayMusic();
        }
    }

    public void PlayMusic(bool loop = true)
    {
        if (backgroundMusic.clip != null)
        {
            backgroundMusic.loop = loop;
            backgroundMusic.Play();
        }
    }

    public void ChangeMusic(AudioClip newClip)
    {
        if (newClip != null)
        {
            if (!newClip.loadState.Equals(AudioDataLoadState.Loaded))
            {
                newClip.LoadAudioData();
            }
            backgroundMusic.Stop();
            backgroundMusic.clip = newClip;
            backgroundMusic.volume = backgroundMusicVolume;
            backgroundMusic.Play();
        }
    }


    public void SetMusicVolume(float volume)
    {
        backgroundMusicVolume = volume;
        backgroundMusic.volume = volume;
    }
}
