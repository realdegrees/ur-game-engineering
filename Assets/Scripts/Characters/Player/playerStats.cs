using System;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private float maxHealthBarWidth;

    protected override void Start()
    {
        base.Start();
        maxHealthBarWidth = UIManager.Instance.healthBarIcon.rectTransform.localScale.x;
    }

    protected override void OnHealthChanged()
    {
        UIManager.Instance.healthBarIcon.rectTransform.localScale = new Vector3(
            maxHealthBarWidth * ((float)health / (float)maxHealth),
            UIManager.Instance.healthBarIcon.rectTransform.localScale.y,
            UIManager.Instance.healthBarIcon.rectTransform.localScale.z
        );
    }
}