using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioClip newMusicClip;
    public bool loop = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && AudioManager.instance != null)
        {
            Debug.Log("ENTERED");
            AudioManager.instance.ChangeMusic(newMusicClip, loop);
        }
    }
}