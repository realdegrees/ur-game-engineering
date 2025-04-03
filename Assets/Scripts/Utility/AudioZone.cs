using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioZone : EditorZone<AudioZone>
{
    private AudioSource audioSource;
    public List<AudioClip> audios = new();
    public float transitionDuration = .6f;
    private Coroutine transitionCoroutine;
    private float defaultVolume = 1f;

    #region Zone Events

    protected override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>();

        audioSource.loop = false;
        defaultVolume = audioSource.volume;
        OnActivate.AddListener((go) =>
        {
            StartTransition(audioSource.volume, defaultVolume);
            AudioManager.Instance.StopMusic();
            Debug.Log("AudioZone activated: " + gameObject.name);
        });
        OnDeactivate.AddListener(() =>
        {
            StartTransition(audioSource.volume, 0);
            AudioManager.Instance.StartMusic();
            Debug.Log("AudioZone deactivated: " + gameObject.name);
        });
    }
    private void StartTransition(float from, float to)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionMusic(from, to, transitionDuration));
    }

    private void Update()
    {
        if (!audioSource.isPlaying && audioSource.volume > 0 && inZone.Count > 0)
        {
            AudioClip newClip = null;
            while (audios.Count > 1 && (newClip == null || newClip == audioSource.clip))
            {
                newClip = audios[Random.Range(0, audios.Count)];
            }

            audioSource.clip = newClip;
            audioSource.Play();
        }
    }
    private IEnumerator TransitionMusic(float from, float to, float duration)
    {
        if (from == 0)
        {
            audioSource.clip = audios[Random.Range(0, audios.Count)];
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
            audioSource.Stop();
        }
    }

    #endregion
}
