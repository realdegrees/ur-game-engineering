using UnityEngine;
using System.Collections;
using System.Diagnostics;

[RequireComponent(typeof(AudioSource))]
public class DamageZone : EditorZone<DamageZone>
{
    public int damage;
    public bool isContinuousSpikes;
    public AudioClip trapAudio;

    public float trapSpeed = 0.2f;
    public float movementDuration = 0.5f;
    public float idleDuration = 1.5f;

    private bool spikesShowing = false;
    private bool hasDamagedPlayer = false;

    private Animator animator;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (isContinuousSpikes)
        {
            StartCoroutine(SpikeCycle());
        }
    }

    void Update()
    {
        if (isContinuousSpikes && spikesShowing && !hasDamagedPlayer)
        {
            inZone.ForEach((go) =>
            {
                if (!go.TryGetComponent(out CharacterStats characterStats)) return;
                characterStats.TakeDamage(damage);
                hasDamagedPlayer = true;
            });
        }
    }

    private IEnumerator SpikeCycle()
    {
        while (true)
        {
            animator.Play("Up");
            audioSource.PlayOneShot(trapAudio);
            spikesShowing = true;
            yield return new WaitForSeconds(movementDuration);

            animator.Play("IdleUp");
            yield return new WaitForSeconds(idleDuration);

            animator.Play("Down");
            spikesShowing = false;
            hasDamagedPlayer = false;
            yield return new WaitForSeconds(movementDuration);

            animator.Play("IdleDown");
            yield return new WaitForSeconds(idleDuration);
        }
    }
}
