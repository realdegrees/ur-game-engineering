using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class DamageZone : EditorZone<DamageZone>
{
    public int damage;
    public AudioClip trapAudio;

    public float stayUpDuration = 0.5f;
    public float stayDownDuration = 3f;

    private Animator animator;
    private AudioSource audioSource;

    private bool damageApplied = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(DamageCycle());

    }

    private void Update()
    {
        if (!damageApplied && animator.GetCurrentAnimatorStateInfo(0).IsName("Up") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
        {
            inZone.ForEach((go) =>
            {
                if (!go.TryGetComponent(out CharacterStats characterStats)) return;
                characterStats.TakeDamage(damage);
            });
            damageApplied = true;
        }
    }

    private IEnumerator DamageCycle()
    {
        while (true)
        {
            animator.Play("Up");
            if (trapAudio != null) audioSource.PlayOneShot(trapAudio);
            yield return new WaitForSeconds(stayUpDuration);
            animator.Play("Down");
            yield return new WaitForSeconds(stayDownDuration);
            damageApplied = false;
        }
    }
}
