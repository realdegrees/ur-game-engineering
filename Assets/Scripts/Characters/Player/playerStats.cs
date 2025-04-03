using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private readonly List<Item> items = new();
    public int potionHealAmount = 30;
    private void Start()
    {
        UIManager.Instance.inventoryKeysText.text = "0";
        UIManager.Instance.healthBar.maxValue = maxHealth;
        UIManager.Instance.healthBar.minValue = 0;
        UIManager.Instance.healthBar.value = maxHealth;

        Coroutine healthBarCoroutine = null;

        OnHealthChanged.AddListener((health) =>
        {
            if (healthBarCoroutine != null)
            {
                StopCoroutine(healthBarCoroutine);
            }
            healthBarCoroutine = StartCoroutine(SmoothUpdateHealthBar(health));
        });
    }
    IEnumerator SmoothUpdateHealthBar(int health)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        float startValue = UIManager.Instance.healthBar.value;
        float targetValue = health;

        while (elapsedTime < duration)
        {
            UIManager.Instance.healthBar.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        UIManager.Instance.healthBar.value = targetValue;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var item = collision.GetComponent<PickupableItem>();
        if (item == null || item.isPickupable == false)
        {
            return;
        }
        if (item.GetItem().type == EItemType.POTION)
        {
            Heal((uint)potionHealAmount);
            item.Pickup();
            return;
        }
        items.Add(item.GetItem());
        item.Pickup();
        UIManager.Instance.inventoryKeysText.text = items.Where(item => item.type == EItemType.KEY).ToArray().Length.ToString();

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