using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class BrokenFloor : MonoBehaviour
{

    public Sprite[] breakStages;
    public float breakTime = 1.5f;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActivated)
        {
            StartCoroutine(BreakFloor());
            isActivated = true;
        }
    }
    
    private IEnumerator BreakFloor()
    {
        for (int i = 0; i < breakStages.Length; i++)
        {
            spriteRenderer.sprite = breakStages[i];
            yield return new WaitForSeconds(breakTime);
        }

        Destroy(gameObject);
    }
}