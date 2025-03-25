using UnityEngine;

public class AudioTrigger : EditorZone<DialogueTrigger>
{
    [Header("Volume Settings")]
    [SerializeField]
    [Range(0f, 1f)] float volume;

    public bool changeBgMusic = false;
    public bool stopBgMusic = false;
    public AudioClip audioClip;

    protected override void Start()
    {
        base.Start();
        OnActivate.AddListener(OnAudioTriggerActivated);
    }

    private void OnAudioTriggerActivated()
    {
        if (changeBgMusic)
        {
            AudioManager.Instance.SetMusicVolume(volume);
            AudioManager.Instance.ChangeMusic(audioClip);
        }
        else
        {
            AudioManager.Instance.SetSoundEffectVolume(volume);
            AudioManager.Instance.PlayAudioClip(audioClip);
        }

        if (stopBgMusic)
        {
            AudioManager.Instance.StopMusic();
        }
    }
}
