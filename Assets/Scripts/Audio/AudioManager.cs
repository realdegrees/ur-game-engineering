using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Manager<AudioManager>
{

    [Header("Background Music")]
    public AudioClip backgroundAudio;
    private AudioSource audioSource;
    private float defaultBackgroundMusicVolume = 1f;
    public float transitionDuration = .6f;
    private Coroutine transitionCoroutine;
    private readonly HashSet<AudioZone> stoppers = new();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = backgroundAudio;
        defaultBackgroundMusicVolume = audioSource.volume;
        audioSource.Play();
    }


    public void StopMusic(AudioZone source)
    {
        stoppers.Add(source);
        if (stoppers.Count <= 1)
            StartTransition(audioSource.volume, 0);
    }

    public void StartMusic(AudioZone source)
    {
        stoppers.Remove(source);
        if (stoppers.Count <= 0 && audioSource)
            StartTransition(audioSource.volume, defaultBackgroundMusicVolume);
    }



    private void StartTransition(float from, float to)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionMusic(from, to, transitionDuration));
    }
    private IEnumerator TransitionMusic(float from, float to, float duration)
    {
        if (from == 0)
        {
            audioSource.UnPause();
        }
        float elapsed = 0f;
        audioSource.volume = from;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            audioSource.volume = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = to;
        if (to == 0)
        {
            audioSource.Pause();
        }
    }
}
