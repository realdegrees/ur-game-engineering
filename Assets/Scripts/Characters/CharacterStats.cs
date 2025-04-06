using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public abstract class CharacterStats : MonoBehaviour
{

    protected int health;
    protected AudioSource audioSource;

    [SerializeField]
    protected List<AudioClip> takeDamageAudios;
    [SerializeField]
    protected List<AudioClip> deathAudios;

    public readonly UnityEvent<int> OnHealthChanged = new();

    public int damage;
    public int maxHealth = 100;

    private float pitch = 1f;

    protected virtual void Awake()
    {
        health = maxHealth;
        TryGetComponent(out audioSource);
        pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.pitch = pitch;
    }

    public void Heal(uint heal)
    {
        health = Mathf.Min(health + (int)heal, maxHealth);
        OnHealthChanged.Invoke(health);
    }


    public int GetHealth()
    {
        return health;
    }



    public void TakeDamage(int damageTaken)
    {
        if (health <= 0) return;
        health -= damageTaken;
        health = Math.Max(Math.Min(health, maxHealth), 0);
        OnHealthChanged.Invoke(health);
        if (health <= 0)
        {
            audioSource.PlayOneShot(deathAudios[UnityEngine.Random.Range(0, deathAudios.Count)]);
        }
        else
        {
            audioSource.PlayOneShot(takeDamageAudios[UnityEngine.Random.Range(0, takeDamageAudios.Count)]);
        }
    }
}