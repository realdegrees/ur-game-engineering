using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Spikes : EditorZone<Spikes>
{
    private Animator animator;
    public Collider2D blockVolume;

    public bool IsOpen => animator.GetCurrentAnimatorStateInfo(0).IsName("Open");
    protected override void Start()
    {
        base.Start();

        TryGetComponent(out animator);
        OnActivate.AddListener(() =>
        {
            animator.SetBool("Open", true);
            blockVolume.enabled = false;
        });
        OnDeactivate.AddListener(() =>
        {
            animator.SetBool("Open", false);
            blockVolume.enabled = true;
        });
    }
}
