using System.Linq;
using UnityEngine;

public class CompanionController : NPCController
{
    protected override void Start()
    {
        base.Start();
        if (GameManager.Instance.Scenario == "B")
        {
            attacksTags = new() { };
            followsTags = new() { };
        }
        stateMachine.OnTargetChanged += (target) =>
        {
            if (target == null)
            {
                stateMachine.SetTarget(PlayerController.Instance.transform);
            }
        };
    }
    protected override void Update()
    {
        base.Update();
        if (GameManager.Instance.Scenario == "B")
        {
            bool enemiesInRange = Physics2D.OverlapCircleAll(stateMachine.rb.position, stateMachine.Config.FollowDistance).Any(h => h.transform.root.CompareTag("Hostile"));
            animator.SetBool("Scared", enemiesInRange);
        }
    }
}