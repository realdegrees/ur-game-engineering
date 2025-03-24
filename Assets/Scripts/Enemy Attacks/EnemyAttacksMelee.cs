// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EnemyAttacksMelee : MonoBehaviour
// {

//     [SerializeField] int damage;
//     public AudioClip[] swordSounds;
//     public float attackCooldown = 1.5f;

//     private AudioSource audioSource;
//     private PlayerStats playerStats;
//     private GameObject player;

//     void Start()
//     {
//         player = GameObject.FindWithTag("Player");
//         audioSource = GetComponent<AudioSource>();
//         playerStats = player.GetComponent<PlayerStats>();
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("PlayerBody"))
//         {
//            // Attack();   
//            //StartCoroutine(Attack);
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("PlayerBody"))
//         {
//            // Attack();   
//            //StopCoroutine(Attack);      
//         }
//     }

//     // private void Attack()
//     // {
//     //     playerStats.TakeDamage(damage);
//     //     PlayRandomSwordSound();
//     // }

//     private void PlayRandomSwordSound()
//     {
//         if (swordSounds.Length > 0)
//         {
//             AudioClip selectedSound = swordSounds[Random.Range(0, swordSounds.Length)];
//             audioSource.PlayOneShot(selectedSound);
//         }
//     }

//     // private IEnumerator Attack() {
//     //     while(true)
//     //     {
//     //         playerStats.TakeDamage(damage);
//     //         PlayRandomSwordSound();
//     //         WaitForSeconds(attackCooldown);
//     //     }
//     // }
// }