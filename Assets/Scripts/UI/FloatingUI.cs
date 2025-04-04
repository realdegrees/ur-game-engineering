
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class FloatingUI : MonoBehaviour
{
    public CharacterStats linkToStats;
    public Slider healthBar;
    private Canvas canvas;

    private void Start()
    {
        TryGetComponent(out canvas);
        if (linkToStats == null)
        {
            Debug.Log("FloatingHealthbar: No link to stats set");
            return;
        }
        if (healthBar == null)
        {
            Debug.Log("FloatingHealthbar: No health bar icon set");
            return;
        }
        healthBar.maxValue = linkToStats.maxHealth;
        healthBar.minValue = 0;
        healthBar.value = linkToStats.GetHealth();
        linkToStats.OnHealthChanged.AddListener(health =>
        {
            if (health <= 0)
            {
                canvas.enabled = false;
                return;
            }
            healthBar.value = health;
        });
    }

    public void Update()
    {
        // always keep the canvas rotate upwards in world space
        canvas.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        if (linkToStats.transform.rotation.eulerAngles.y != 0)
        {
            canvas.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            canvas.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}