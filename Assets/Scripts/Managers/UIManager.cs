using UnityEngine;
using System;
using Manager;
using UnityEngine.UI;


public class UIManager : Manager<UIManager>
{
    [Header("Inventory UI")]
    public Image inventoryKeysIcon;
    public Text inventoryKeysText;
    public Image inventoryPotionIcon;
    public Text inventoryPotionText;
    public Image healthBarIcon;
}
