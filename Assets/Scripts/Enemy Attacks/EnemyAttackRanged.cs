using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRanged : MonoBehaviour
{

    [SerializeField] int damage;
    [SerializeField] GameObject projectile;

    public Transform projectilePos;
    public AudioClip[] arrowSounds;

    private float spawnTimer;  
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
        PlayRandomArrowSound();
    }

    private void PlayRandomArrowSound()
    {
        if (arrowSounds.Length > 0)
        {
            AudioClip selectedSound = arrowSounds[Random.Range(0, arrowSounds.Length)];
            audioSource.PlayOneShot(selectedSound);
        }
    }
}
