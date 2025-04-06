using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    [SerializeField]
    private Item item;
    public bool isPickupable = true;

    public Item GetItem()
    {
        return item;
    }

    public void Pickup()
    {
        Destroy(this.gameObject);
    }
}