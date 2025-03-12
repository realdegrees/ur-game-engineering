using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    [SerializeField]
    private Item item;
    public Item GetItem()
    {
        return item;
    }

    public void Pickup()
    {
        Destroy(this.gameObject);
    }
}