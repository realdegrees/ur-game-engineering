using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterStateMachine))]
public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private List<Item> items = new();
    private CharacterStateMachine stateMachine;


    void Start()
    {
        TryGetComponent<CharacterStateMachine>(out stateMachine);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var item = collision.GetComponent<PickupableItem>();
        if (item == null)
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