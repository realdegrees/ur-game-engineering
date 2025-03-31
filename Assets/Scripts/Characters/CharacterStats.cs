using System;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{

    protected int health;

    public int damage;
    [SerializeField]
    protected int maxHealth = 100;

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

    protected virtual void Start()
    {
        health = maxHealth;
    }

    protected abstract void OnHealthChanged();




    public int TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        health = Math.Max(Math.Min(health, maxHealth), 0);
        OnHealthChanged();
        return health;
    }
}