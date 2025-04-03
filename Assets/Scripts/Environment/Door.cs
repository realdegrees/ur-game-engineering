using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Door : EditorZone<Door>
{
    private Animator animator;
    public Collider2D blockVolume;

    public int requiredKeys = 0;

    protected override void Start()
    {
        base.Start();

        TryGetComponent(out animator);
        OnActivate.AddListener((go) =>
        {
            if (requiredKeys > 0 && go.TryGetComponent<PlayerStats>(out var inventory))
            {
                if (inventory.GetItems(EItemType.KEY).Count < requiredKeys)
                {
                    return;
                }
                inventory.RemoveItems(EItemType.KEY, requiredKeys);
                requiredKeys = 0;
            }
            else if (requiredKeys > 0)
            {
                return;
            }
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