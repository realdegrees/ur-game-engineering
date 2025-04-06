using UnityEngine;
using System;
using Manager;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class UIManager : Manager<UIManager>
{
    [Header("Inventory UI")]
    public Image inventoryKeysIcon;
    public TextMeshProUGUI inventoryKeysText;
    public Slider healthBar;
    public Image deathBackground;
    public TextMeshProUGUI deathText;

    void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 0f);
            deathBackground.color = new Color(deathBackground.color.r, deathBackground.color.g, deathBackground.color.b, 0f);
        };
    }
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
