using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class DamageZone : EditorZone<DamageZone>
{

    public int damage;
    public GameObject trap;

    public bool isHiddenSpikes;
    public bool isRocks;
    public bool isContinuousSpikes;

    public float trapSpeed = 0.2f;
    public float continuousSpikesTime = 1.5f;
    private Vector2 originalTrapPos;
    private PlayerStats playerStats;

    private bool spikesShowing = false;
    private bool hasDamagedPlayer = false;

    protected override void Start()
    {
        base.Start();
        playerStats = playerStateMachine.GetComponentInParent<PlayerStats>();
        originalTrapPos = trap.transform.position;
        if (isContinuousSpikes)
        {
            Vector2 target = new(transform.position.x, transform.position.y + 0.2f);
            StartCoroutine(MoveSpikes(target));
        }
        else if (isHiddenSpikes)
        {
            Vector2 target = new(originalTrapPos.x, originalTrapPos.y - 0.2f);
            OnDeactivate.AddListener(() => StartCoroutine(HideTrap()));
        }

        if (!isContinuousSpikes)
        {
            OnActivate.AddListener(() =>
            {
                DealDamage();
            });
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (isContinuousSpikes && spikesShowing && !hasDamagedPlayer)
        {
            if (other.transform.root.TryGetComponent(out PlayerController controller))
            {
                if (other == controller.stateMachine.bodyCollider)
                {
                   // DealDamage();
                //    if (spikesShowing)
                //    {
                //         playerStats.TakeDamage(damage);
                //         spikesShowing = false;
                //    }
                playerStats.TakeDamage(damage);
                hasDamagedPlayer = true;
                }
            }
        }
    }

    public void DealDamage(GameObject go)
    {
        if (isHiddenSpikes || isRocks)
        {
            Vector2 target = originalTrapPos;
            if (isHiddenSpikes)
            {
                target = new Vector2(originalTrapPos.x, originalTrapPos.y + 0.2f);
            }
            else if (isRocks)
            {
                target = new Vector2(originalTrapPos.x, originalTrapPos.y - 5f);
            }
            playerStats.TakeDamage(damage);
            StartCoroutine(ActivateTrap(target));
        }

        // else if (isContinuousSpikes)
        // {
        //     if (spikesShowing) playerStats.TakeDamage(damage);
        // }
    }

    private IEnumerator MoveSpikes(Vector2 target)
    {
        while (true)
        {
            yield return StartCoroutine(ActivateTrap(target));
            yield return new WaitForSeconds(continuousSpikesTime);
            spikesShowing = false;
            hasDamagedPlayer = false;

            yield return StartCoroutine(HideTrap());
            yield return new WaitForSeconds(continuousSpikesTime);
            spikesShowing = true;
        }
    }

    private IEnumerator ActivateTrap(Vector2 target)
    {
        Vector2 start = originalTrapPos;
        if (isContinuousSpikes) start = transform.position;
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
        if (isContinuousSpikes) start = transform.position;
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
