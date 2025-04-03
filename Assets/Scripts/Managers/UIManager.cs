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
    public Slider healthBar;

    public void Disable()
    {
        foreach (var canvas in inventoryKeysIcon.transform.root.GetComponentsInChildren<Canvas>())
        {
            canvas.enabled = false;
        }
    }
    public void Enable()
    {
        foreach (var canvas in inventoryKeysIcon.transform.root.GetComponentsInChildren<Canvas>())
        {
            canvas.enabled = true;
        }
    }
}
