using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class BrokenFloorZone : EditorZone<BrokenFloorZone>
{

    public Sprite[] breakStages;
    public float breakTime = 1.5f;

    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        OnActivate.AddListener(() =>
        {
            StartCoroutine(BreakFloor());
        });
    }
    
    private IEnumerator BreakFloor()
    {
        for (int i = 0; i < breakStages.Length; i++)
        {
            spriteRenderer.sprite = breakStages[i];
            yield return new WaitForSeconds(breakTime);
        }

        Destroy(gameObject);
    }
}