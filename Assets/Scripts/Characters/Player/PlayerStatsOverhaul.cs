using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsOverhaul : MonoBehaviour
{

    public int maxHealth = 20;
    public int health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Health: " + health);
        if (health <= 0)
        {
            Debug.Log("Player died");
        }
    }
}
