
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class FloatingUI : MonoBehaviour
{
    public CharacterStats linkToStats;
    public Image healthBarIcon;
    private Canvas canvas;

    private void Start()
    {
        TryGetComponent(out canvas);
        if (linkToStats == null)
        {
            Debug.Log("FloatingHealthbar: No link to stats set");
            return;
        }
        if (healthBarIcon == null)
        {
            Debug.Log("FloatingHealthbar: No health bar icon set");
            return;
        }
        var maxHealthBarWidth = healthBarIcon.rectTransform.localScale.x;
        linkToStats.OnHealthChanged += health =>
        {
            if (health <= 0)
            {
                canvas.enabled = false;
                return;
            }
            healthBarIcon.rectTransform.localScale = new Vector3(
                            maxHealthBarWidth * ((float)health / (float)linkToStats.maxHealth),
                            healthBarIcon.rectTransform.localScale.y,
                            healthBarIcon.rectTransform.localScale.z
                        );
        };
    }

    public void Update()
    {
        // always keep the canvas rotate upwards in world space
        canvas.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        if (linkToStats.transform.rotation.eulerAngles.y == 180)
        {
            canvas.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}