using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    [SerializeField] int damage;
    public float speed;
    public AudioClip[] arrowSounds;

    private float timer;
    private Rigidbody2D rb;
    private GameObject player;
    private PlayerStats playerStats;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();

        Vector3 direction = player.transform.position - transform.position;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * speed;
        PlayRandomArrowSound();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 5)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            playerStats.TakeDamage(damage);
        }
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