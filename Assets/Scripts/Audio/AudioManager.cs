using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class AudioManager : Manager<AudioManager>
{

    [Header("Background Music")]
    public AudioClip backgroundMusicClip;
    private AudioSource backgroundMusic;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float backgroundMusicVolume = 0.5f;
    [Range(0f, 1f)] public float duckingFactor = 0.3f;

    private AudioSource soundEffect;
    private float soundEffectVolume = 0.5f;

    // public static AudioManager instance { get; private set; }

    // private void Awake()
    // {
    //     if (instance != null)
    //     {
    //         Debug.Log("AudioManager already exists in the scene");
    //     }
    //     instance = this;
    // }

    void Start()
    {
        backgroundMusic = gameObject.AddComponent<AudioSource>();
        soundEffect = gameObject.AddComponent<AudioSource>();
        backgroundMusic.clip = backgroundMusicClip;
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
            if (backgroundMusic.clip != null) {
                backgroundMusic.Stop();
            }
            backgroundMusic.clip = newClip;
            backgroundMusic.volume = backgroundMusicVolume;
            backgroundMusic.Play();
        }
    }

    public void StopMusic()
    {
        backgroundMusic.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        backgroundMusicVolume = volume;
        backgroundMusic.volume = volume;
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            audioClip.LoadAudioData();
        }
        soundEffect.clip = audioClip;
        soundEffect.volume = soundEffectVolume;
        soundEffect.Play();
    }

    public void SetSoundEffectVolume(float volume)
    {
        soundEffectVolume = volume;
        soundEffect.volume = volume;
    }
}
