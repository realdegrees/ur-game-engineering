using UnityEngine;
using System;
using Manager;
using UnityEngine.UI;
using TMPro;


public class UIManager : Manager<UIManager>
{
    [Header("Inventory UI")]
    public Image inventoryKeysIcon;
    public TextMeshProUGUI inventoryKeysText;
    public Image inventoryPotionIcon;
    public TextMeshProUGUI inventoryPotionText;
    public Image healthBarIcon;
}
