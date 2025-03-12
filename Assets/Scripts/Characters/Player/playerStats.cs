using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    private int health;

    private int damage;
    private float maxHealthBarWidth;
    [SerializeField]
    private int maxHealth = 100;

    public int Heal(int heal)
    {
        this.health += heal;
        return this.health;
    }


    public int GetHealth()
    {
        return this.health;
    }

    public int SetHealth(int newHealthStat)
    {
        this.health = Math.Max(Math.Min(newHealthStat, maxHealth), 0);
        OnHealthChanged();
        return health;
    }

    void Start()
    {
        maxHealthBarWidth = UIManager.Instance.healthBarIcon.rectTransform.localScale.x;
        health = maxHealth;

    }

    private void OnHealthChanged()
    {

        UIManager.Instance.healthBarIcon.rectTransform.localScale = new Vector3(
            maxHealthBarWidth * ((float)health / (float)maxHealth),
            UIManager.Instance.healthBarIcon.rectTransform.localScale.y,
            UIManager.Instance.healthBarIcon.rectTransform.localScale.z
        );
    }


    public int TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        this.health = Math.Max(Math.Min(health, maxHealth), 0);
        OnHealthChanged();
        return health;
    }

    public int GetDamage()
    {
        return damage;
    }


}