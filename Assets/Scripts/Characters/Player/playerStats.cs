using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private float maxHealthBarWidth;
    private readonly List<Item> items = new();

    protected override void Start()
    {
        base.Start();
        maxHealthBarWidth = UIManager.Instance.healthBarIcon.rectTransform.localScale.x;
        OnHealthChanged += health =>
        {
            UIManager.Instance.healthBarIcon.rectTransform.localScale = new Vector3(
                maxHealthBarWidth * ((float)health / (float)maxHealth),
                UIManager.Instance.healthBarIcon.rectTransform.localScale.y,
                UIManager.Instance.healthBarIcon.rectTransform.localScale.z
            );
        };
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var item = collision.GetComponent<PickupableItem>();
        if (item == null || item.isPickupable == false)
        {
            return;
        }
        items.Add(item.GetItem());
        item.Pickup();
        UIManager.Instance.inventoryKeysText.text = items.Where(item => item.type == EItemType.KEY).ToArray().Length.ToString();
        UIManager.Instance.inventoryPotionText.text = items.Where(item => item.type == EItemType.POTION).ToArray().Length.ToString();

    }
    public List<Item> GetItems(EItemType type)
    {
        return items.Where(item => item.type == type).ToList();
    }
    public void RemoveItems(EItemType type, int amount)
    {
        var itemsToRemove = items.Where(item => item.type == type).Take(amount).ToList();
        foreach (var item in itemsToRemove)
        {
            items.Remove(item);
        }
    }
}