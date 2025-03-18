using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacksMelee : MonoBehaviour
{

    [SerializeField] int damage;

    public PlayerStatsOverhaul playerStats;
    public AudioClip[] swordSounds;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            Attack();            
        }
    }

    private void Attack()
    {
        playerStats.TakeDamage(damage);
        PlayRandomSwordSound();
        Debug.Log("attacked");
    }

    private void PlayRandomSwordSound()
    {
        if (swordSounds.Length > 0)
        {
            AudioClip selectedSound = swordSounds[Random.Range(0, swordSounds.Length)];
            audioSource.PlayOneShot(selectedSound);
        }
    }
}