using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{

    [SerializeField] int damage;

    public PlayerStatsOverhaul playerStats;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            //collision.transform.root.GetComponent<PlayerStatsOverhaul>().health -= damage;
            //Debug.Log(collision.transform.root.GetComponent<PlayerStatsOverhaul>().health);
            playerStats.TakeDamage(damage);
            Debug.Log("attacked");
        }
    }
}
