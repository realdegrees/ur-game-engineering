using UnityEngine;

public class BrokenFloor : MonoBehaviour
{
    public Sprite[] breakStages;

    private SpriteRenderer spriteRenderer;
    private int currentStage = 0;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentStage < breakStages.Length)
        {
            AdvanceBreakStage();
        }
    }

    private void AdvanceBreakStage()
    {
        spriteRenderer.sprite = breakStages[currentStage];
        currentStage++;

        if (currentStage >= breakStages.Length)
        {
            Destroy(gameObject);
        }
    }
}