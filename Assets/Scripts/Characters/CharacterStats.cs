using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class CharacterStats : MonoBehaviour
{

    protected int health;
    protected AudioSource audioSource;

    [SerializeField]
    protected List<AudioClip> takeDamageAudios;
    [SerializeField]
    protected List<AudioClip> deathAudios;

    public int damage;
    [SerializeField]
    protected int maxHealth = 100;

    private float pitch = 1f;

    protected virtual void Start()
    {
        health = maxHealth;
        TryGetComponent(out audioSource);
        pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.pitch = pitch;
    }

    public int Heal(int heal)
    {
        health += heal;
        return health;
    }


    public int GetHealth()
    {
        return health;
    }

    public int SetHealth(int newHealthStat)
    {
        health = Math.Max(Math.Min(newHealthStat, maxHealth), 0);
        OnHealthChanged();
        return health;
    }



    protected abstract void OnHealthChanged();




    public int TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        health = Math.Max(Math.Min(health, maxHealth), 0);
        OnHealthChanged();
        if (health <= 0)
        {
            audioSource.PlayOneShot(deathAudios[UnityEngine.Random.Range(0, deathAudios.Count)]);
        }
        else
        {
            audioSource.PlayOneShot(takeDamageAudios[UnityEngine.Random.Range(0, takeDamageAudios.Count)]);
        }
        return health;
    }
}