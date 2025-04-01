using System.Collections;
using UnityEngine;

public class NPCStats : CharacterStats
{
    protected override void Start()
    {
        base.Start();
        OnHealthChanged += OnChangeHealthInvoked;
    }
    private void OnChangeHealthInvoked(int health)
    {
        if (health <= 0)
        {
            // disable the statemachine component and set rigidbody freezerotation to false, then apply a rotation force backwards to the rigidbody
            var npcStateMachine = GetComponent<NPCStateMachine>();
            var npcController = GetComponent<NPCController>();
            npcStateMachine.enabled = false;
            npcController.enabled = false;
            var rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
            rigidbody.gravityScale = 2f;
            GetComponent<Animator>().enabled = false;
            rigidbody.AddTorque(1f, ForceMode2D.Impulse);
            // set the layer of all children recursively to dead
            gameObject.layer = LayerMask.NameToLayer("Dead");
            SetChildrenLayer(gameObject.transform, LayerMask.NameToLayer("Dead"));
            // after 3 seconds start shrinking the gameobject over a span of 5 seconds and destroy it afterwards
            StartCoroutine(ShrinkAndDestroy());
            enabled = false;
        }
    }

    IEnumerator ShrinkAndDestroy()
    {
        float duration = 3f;
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;

        yield return new WaitForSeconds(2f); // Wait for 3 seconds before starting the shrink
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }

    private void SetChildrenLayer(Transform parent, int layer)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.layer = layer;
            SetChildrenLayer(child, layer);
        }
    }
}