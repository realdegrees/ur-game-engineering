using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class DamageZone : EditorZone<DamageZone>
{
    public int damage;

    public bool isRocks;
    public bool isContinuousSpikes;

    public float trapSpeed = 0.2f;
    public float movementDuration = 0.5f;
    public float idleDuration = 1.5f;

    private bool spikesShowing = false;
    private bool hasDamagedPlayer = false;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();

        if (isContinuousSpikes)
        {
            StartCoroutine(SpikeCycle());
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        var tag = other.transform.root.tag;

        if (isContinuousSpikes && spikesShowing && !hasDamagedPlayer)
        {
            inZone.ForEach((go) =>
            {
                if (!other.transform.root.TryGetComponent(out PlayerStats playerStats)) return;
                playerStats.TakeDamage(damage);
                hasDamagedPlayer = true;
            });
        }
    }

    private IEnumerator SpikeCycle()
    {
        while (true)
        {
            animator.Play("Up");
            yield return new WaitForSeconds(movementDuration);

            animator.Play("IdleUp");
            yield return new WaitForSeconds(idleDuration);

            animator.Play("Down");
            yield return new WaitForSeconds(movementDuration);

            animator.Play("IdleDown");
            yield return new WaitForSeconds(idleDuration);
        }
    }
}
