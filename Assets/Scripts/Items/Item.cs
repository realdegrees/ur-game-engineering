using UnityEditor;
using UnityEngine;

public enum EItemType
{
    KEY, POTION, LANTERN,

}
[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public EItemType type;

    // public Item(string name, string description)
    // {
    //     Name = name;
    //     Description = description;
    // }


    // public string getName()
    // {
    //     return Name;
    // }

    // public string getDescription()
    // {
    //     return Description;
    // }
    // public abstract void Use(PlayerStats player);
}

// public class HealthPotion : Item
// {
//     public int healAmount;


//     public HealthPotion(int healAmount)
//         : base("Health Potion", "Stellt Gesundheit wieder her.")
//     {
//         this.healAmount = healAmount;
//     }

//     public override void Use(PlayerStats player)
//     {
//         player.SetHealth(player.GetHealth() + healAmount);

//         player.RemoveItem(this);
//     }
// }

// public class Key : Item
// {


//     public Key()
//         : base("Key", "Öffnet eine Tür oder Truhe.")
//     { }

//     public override void Use(PlayerStats player)
//     {
//         player.RemoveItem(this);
//     }
// }