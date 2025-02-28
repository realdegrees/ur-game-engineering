using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRanged : MonoBehaviour
{

    [SerializeField] int damage;
    [SerializeField] GameObject projectile;

    public Transform projectilePos;
    public PlayerStatsOverhaul playerStats;

    private float spawnTimer;

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > 2)
        {
            spawnTimer = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        Instantiate(projectile, projectilePos.position, Quaternion.identity);

    }
}
