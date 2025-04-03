using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Utility;
public class ProjectileScript : MonoBehaviour
{
    [HideInInspector]
    public int damage;
    public float speed;
    public bool spin = false;
    public int rotationOffset = 90;
    public bool stickOnImpact = false;


    [Tooltip("Tags that the projectile will ignore during collision. The character that fires the projectile is automatically added to this list.")]
    public List<string> ignoresTags = new() { };

    private float timer;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Init(Vector2 direction)
    {
        rb.velocity = direction.normalized * speed;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 5)
        {
            Destroy(gameObject);
        }
        if (rb.isKinematic) return;
        if (!spin)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, rb.velocity) * Quaternion.Euler(0, 0, rotationOffset);
        }
        else
        {
            transform.Rotate(Vector3.forward * 150 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (rb.isKinematic) return;
        var ignore = ignoresTags.FirstOrDefault((t) =>
        {
            return Util.FindParentWithTag(other.transform, t) != null;
        });
        if (ignore != null || other.isTrigger) return;
        var stats = other.GetComponentInParent<CharacterStats>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
        }
        if (stickOnImpact)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.transform.SetParent(other.transform);
            rb.position += (Vector2)rb.transform.forward * 0.5f;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}