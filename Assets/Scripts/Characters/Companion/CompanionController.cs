using System.Collections;
using System.Linq;
using Assets.Scripts.Utility;
using UnityEngine;

public class CompanionController : NPCController
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(SetTagsFromScenario());
    }
    IEnumerator SetTagsFromScenario()
    {
        yield return new WaitUntil(() => GameManager.Instance && GameManager.Instance.Scenario != null);
        if (GameManager.Instance.Scenario == "B")
        {
            attacksTags = new() { };
            followsTags = new() { };
        }
    }
    protected override void Update()
    {
        base.Update();
        if (GameManager.Instance.Scenario == "B")
        {
            bool enemiesInRange = Physics2D.OverlapCircleAll(stateMachine.rb.position, stateMachine.Config.ResumeDistance)
                .Any(h =>
                {
                    return Util.FindParentWithTag(h.transform, "Hostile") != null;
                });
            animator.SetBool("Scared", enemiesInRange);
        }
    }
}