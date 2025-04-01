using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Chest : EditorZone<Chest>
{
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip openSound;

    public List<GameObject> items = new();

    protected override void Start()
    {
        base.Start();

        TryGetComponent(out animator);
        TryGetComponent(out audioSource);
        OnActivate.AddListener((go) =>
        {
            if (animator.GetBool(0) == true)
            {
                return;
            }
            animator.SetBool("Open", true);
            audioSource.PlayOneShot(openSound);

            StartCoroutine(SpawnItems());
        });
    }
    private IEnumerator SpawnItems()
    {
        foreach (var item in items)
        {
            var go = Instantiate(item, transform.position, Quaternion.identity);
            if (go.TryGetComponent(out Rigidbody2D rb))
            {

                Vector3 forceDirection = Quaternion.Euler(0, 0, Random.Range(-20f, 20f)) * Vector3.up;
                rb.AddForce(forceDirection.normalized * 8f, ForceMode2D.Impulse);
                //rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
            }

            if (go.TryGetComponent(out PickupableItem pickupable))
            {
                pickupable.isPickupable = false;
                StartCoroutine(EnablePickupableAfterDelay(pickupable, 1f));
            }
            yield return new WaitForSeconds(0.5f);
        }
        items.Clear();
    }
    private IEnumerator EnablePickupableAfterDelay(PickupableItem pickupable, float delay)
    {
        Vector3 originalScale = pickupable.transform.localScale;
        Vector3 startScale = originalScale / 3f;
        pickupable.transform.localScale = startScale;

        float elapsedTime = 0f;
        while (elapsedTime < delay)
        {
            pickupable.transform.localScale = Vector3.Lerp(startScale, originalScale, elapsedTime / delay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pickupable.transform.localScale = originalScale;
        pickupable.isPickupable = true;
    }
}