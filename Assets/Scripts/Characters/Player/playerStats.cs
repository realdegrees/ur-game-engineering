public class PlayerStats
{

    private int health;
    private int stamina;
    private int damage;
    private string[] inventory;


    public PlayerStats(int health, int stamina, int damage, string[] inventory)
    {

        this.health = health;
        this.stamina = stamina;
        this.damage = damage;
        this.inventory = inventory;
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
        return this.health;
    }

    public int GetStamina()
    {
        return this.stamina;
    }

    public int SetStamina(int newStaminaStat)
    {
        this.stamina = newStaminaStat;
        return this.stamina;
    }

    public int TakeDamage(int damageTaken)
    {
        this.health -= damage;
        return this.health;
    }

    public int GetDamage()
    {
        return this.damage;
    }

    public int SetDamage(int newDamageStat)
    {
        this.damage = newDamageStat;
        return this.damage;
    }

    public int DealDamage()
    {
        return this.damage;
    }

    public bool AddItem(string item)
    {
        for (int i = 0; i < this.inventory.Length; i++)
        {
            if (this.inventory[i] == null)
            {
                this.inventory[i] = item;
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(string item)
    {
        for (int i = 0; i < this.inventory.Length; i++)
        {
            if (this.inventory[i] == item)
            {
                this.inventory[i] = null;
                return true;
            }
        }
        return false;
    }

}