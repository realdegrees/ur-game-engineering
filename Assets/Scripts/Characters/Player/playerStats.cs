using System.Collections.Generic;

public class PlayerStats
{

    private int health;
    private int stamina;
    private int damage;
    private List<Item> inventory;


    public PlayerStats(int health, int stamina, int damage, string[] inventory)
    {

        this.health = health;
        this.stamina = stamina;
        this.damage = damage;
        this.inventory = new List<Item>();
    }

    public int Heal(int heal)
    {
        this.health += heal;
        return this.health;
    }

    public int GetHealth()
    {
        return this.health;
    }

    public int SetHealth(int newHealthStat)
    {
        this.health = newHealthStat;
        return health;
    }

    public int GetStamina()
    {
        return stamina;
    }

    public int SetStamina(int newStaminaStat)
    {
        stamina = newStaminaStat;
        return stamina;
    }

    public int TakeDamage(int damageTaken)
    {
        health -= damage;
        return health;
    }

    public int GetDamage()
    {
        return damage;
    }

    public int SetDamage(int newDamageStat)
    {
        damage = newDamageStat;
        return damage;
    }

    public int DealDamage()
    {
        return damage;
    }

    public void AddItem(Item item)
    {
        if (inventory.Count < 3)
        {
            inventory.Add(item);
        }

    }

    public void RemoveItem(Item item)
    {
        if (inventory.Count > 0)
        {
            inventory.Remove(item);
        }

    }

    public void UseItem(Item item)
    {
        if (inventory.Contains(item))
        {
            item.Use(this);
            RemoveItem(item);
        }

    }

}