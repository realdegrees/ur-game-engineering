using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class LavaZone : EditorZone<DamageZone>
{
    public int damage;
    public float interval = 1.0f;


    protected override void Start()
    {
        base.Start();
        StartCoroutine(DamageCycle());

    }

    private IEnumerator DamageCycle()
    {
        while (true)
        {
            DamageEntities();
            yield return new WaitForSeconds(interval);
        }
    }

    private void DamageEntities()
    {
        foreach (var go in inZone)
        {
            if (!go.TryGetComponent(out CharacterStats characterStats)) continue;
            characterStats.TakeDamage(damage);
        }
    }
}
