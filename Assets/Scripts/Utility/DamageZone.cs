using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class DamageZone : EditorZone<DamageZone>
{

    public int damage;
    public GameObject trap;

    public bool isRocks;
    public bool isContinuousSpikes;

    public float trapSpeed = 0.2f;
    public float continuousSpikesTime = 1.5f;
    private Vector2 originalTrapPos;

    private bool spikesShowing = false;
    private bool hasDamagedPlayer = false;

    protected override void Start()
    {
        base.Start();
        originalTrapPos = trap.transform.position;
        if (isContinuousSpikes)
        {
            StartCoroutine(MoveSpikes());
        }

        // if (!isContinuousSpikes)
        // {
        //     OnActivate.AddListener(() =>
        //     {
        //         DealDamage();
        //     });
        // }
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

    // public void DealDamage()
    // {
    //     if (isRocks)
    //     {
    //         Vector2 target = new (originalTrapPos.x, originalTrapPos.y - 5f);
    //         playerStats.TakeDamage(damage);
    //         StartCoroutine(ActivateTrap(target));
    //     }

    // }

    private IEnumerator MoveSpikes()
    {
        while (true)
        {
            yield return StartCoroutine(ActivateTrap());
            yield return new WaitForSeconds(continuousSpikesTime);
            spikesShowing = false;
            hasDamagedPlayer = false;

            yield return StartCoroutine(HideTrap());
            yield return new WaitForSeconds(continuousSpikesTime);
            spikesShowing = true;
        }
    }

    private IEnumerator ActivateTrap()
    {
        Vector2 start = originalTrapPos;
        Vector2 target = new(originalTrapPos.x, originalTrapPos.y + 0.2f);
        //if (isContinuousSpikes) start = transform.position;
        float timePassed = 0f;

        while (timePassed < trapSpeed)
        {
            trap.transform.position = Vector2.Lerp(start, target, timePassed / trapSpeed);
            timePassed += Time.deltaTime;
            yield return null;
        }
        trap.transform.position = target;
        if (numberOfAllowedActivations > 0) Destroy(trap);
    }

    private IEnumerator HideTrap()
    {
        Vector2 start = trap.transform.position;
        //if (isContinuousSpikes) start = transform.position;
        Vector2 target = new(start.x, start.y - 0.2f);
        float timePassed = 0f;

        while (timePassed < trapSpeed)
        {
            trap.transform.position = Vector2.Lerp(start, target, timePassed / trapSpeed);
            timePassed += Time.deltaTime;
            yield return null;
        }
        trap.transform.position = target;
    }
}
