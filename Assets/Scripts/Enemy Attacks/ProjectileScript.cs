using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    public GameObject player;
    [SerializeField] int damage;

    public float speed;

    private float timer;
    private Rigidbody2D rb;
    private PlayerStatsOverhaul playerStats;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStatsOverhaul>();

        Vector3 direction = player.transform.position - transform.position;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * speed;
    }

    // Update is called once per frame
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
            Debug.Log("attacked");
        }
    }
}
