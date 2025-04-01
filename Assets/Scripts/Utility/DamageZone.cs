using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DamageZone : EditorZone<DamageZone>
{
    protected override void Start()
    {
        base.Start();
        OnActivate.AddListener(DealDamage);

    }

    public void DealDamage(GameObject go)
    {
        var player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerStats>().TakeDamage(5);
    }
}
