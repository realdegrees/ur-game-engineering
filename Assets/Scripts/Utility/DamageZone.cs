using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class DamageZone : EditorZone<DamageZone>
{

    public int damage;
    public GameObject trap;

    public bool isSpikes;
    public bool isRocks;

    public float trapSpeed = 0.2f;
    private Vector2 originalTrapPos;

    private PlayerStats playerStats;

    protected override void Start()
    {
        base.Start();
        playerStats = playerStateMachine.GetComponentInParent<PlayerStats>();
        originalTrapPos = trap.transform.position;
        OnActivate.AddListener(() =>
        {
            DealDamage();
        });
        if (isSpikes)
        {
            Vector2 target = new(originalTrapPos.x, originalTrapPos.y - 0.2f);
            OnDeactivate.AddListener(() => StartCoroutine(HideTrap(target)));
        }
    }

    public void DealDamage(GameObject go)
    {
        playerStats.TakeDamage(damage);
        Vector2 target = originalTrapPos;
        if (isSpikes)
        {
            target = new Vector2(originalTrapPos.x, originalTrapPos.y + 0.2f);
        }
        else if (isRocks)
        {
            target = new Vector2(originalTrapPos.x, originalTrapPos.y - 5f);
        }
        StartCoroutine(ActivateTrap(target));
    }

    private IEnumerator ActivateTrap(Vector2 target)
    {
        float timePassed = 0f;

        while (timePassed < trapSpeed)
        {
            trap.transform.position = Vector2.Lerp(originalTrapPos, target, timePassed / trapSpeed);
            timePassed += Time.deltaTime;
            yield return null;
        }
        trap.transform.position = target;
        if (numberOfAllowedActivations > 0) Destroy(trap);
    }

    private IEnumerator HideTrap(Vector2 target)
    {
        //Vector2 start = trap.transform.position;
        // Vector2 target = new(start.x, start.y - 0.2f);
        float timePassed = 0f;

        while (timePassed < trapSpeed)
        {
            trap.transform.position = Vector2.Lerp(trap.transform.position, target, timePassed / trapSpeed);
            timePassed += Time.deltaTime;
            yield return null;
        }
        trap.transform.position = target;
    }

    // private IEnumerator DropRock()
    // {
    //     Vector2 start = originalTrapPos;
    //     Vector2 target = new(start.x, start.y - 5f);
    //     float timePassed = 0f;

    //     while (timePassed < trapSpeed)
    //     {
    //         trap.transform.position = Vector2.Lerp(start, target, timePassed / trapSpeed);
    //         timePassed += Time.deltaTime;
    //         yield return null;
    //     }
    //     trap.transform.position = target;
    //     Destroy(trap);
    // }

    // private IEnumerator ShowSpikes()
    // {
    //     Vector2 start = originalTrapPos;
    //     Vector2 target = new(start.x, start.y + 0.2f);
    //     float timePassed = 0f;

    //     while (timePassed < trapSpeed)
    //     {
    //         trap.transform.position = Vector2.Lerp(start, target, timePassed / trapSpeed);
    //         timePassed += Time.deltaTime;
    //         yield return null;
    //     }
    //     trap.transform.position = target;
    // }
}
