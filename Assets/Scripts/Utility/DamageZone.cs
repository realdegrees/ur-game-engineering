using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DamageZone : EditorZone<DamageZone>
{
    void Start()
    {
        OnActivate.AddListener(DealDamage);

    }

    public void DealDamage()
    {
        var player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerStats>().TakeDamage(5);
    }
}
