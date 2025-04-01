using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class Spikes : EditorZone<Spikes>
{
    private Animator animator;
    public bool isUp = true;

    protected override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
        StartCoroutine(SwitchSpikes());
    }

    IEnumerator SwitchSpikes()
    {
        while (true)
        {
            animator.SetBool("Up", isUp);
            isUp = !isUp;
            
            yield return new WaitForSeconds(1.5f);
        }
    }
}
