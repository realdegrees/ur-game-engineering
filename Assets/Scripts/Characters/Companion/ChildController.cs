using System.Linq;
using UnityEngine;

public class ChildController : NPCController
{
    protected override void Update()
    {
        base.Update();

        bool playerInRange = Physics2D.OverlapCircleAll(stateMachine.rb.position, stateMachine.Config.FollowDistance).Any(h => !h.isTrigger && h.transform.root.CompareTag("Player"));
        animator.SetBool("Crying", !playerInRange);

    }
}