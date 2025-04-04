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
            HandleDeath(health);
        });
    }
    private void HandleDeath(int health)
    {
        if (health > 0)
        {
            return;
        }
        // disable the statemachine component and set rigidbody freezerotation to false, then apply a rotation force backwards to the rigidbody
        var sm = GetComponent<CharacterStateMachine>();
        var playerController = GetComponent<PlayerController>();
        sm.enabled = false;
        playerController.enabled = false;
        var rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
        rigidbody.gravityScale = 2f;
        GetComponent<Animator>().enabled = false;
        rigidbody.AddTorque(1f, ForceMode2D.Impulse);
        // set the layer of all children recursively to dead
        gameObject.layer = LayerMask.NameToLayer("Dead");
        SetChildrenLayer(gameObject.transform, LayerMask.NameToLayer("Dead"));
        // after 3 seconds start shrinking the gameobject over a span of 5 seconds and destroy it afterwards
        StartCoroutine(ShrinkAndDestroy());
        enabled = false;
    }
    IEnumerator ShrinkAndDestroy()
    {
        float duration = 3f;
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;

        yield return new WaitForSeconds(2f); // Wait for 3 seconds before starting the shrink
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }

    private void SetChildrenLayer(Transform parent, int layer)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.layer = layer;
            SetChildrenLayer(child, layer);
        }
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