using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EditorZoneBounds
{
    Mesh,
    Collider
}

[RequireComponent(typeof(Collider2D))]
public abstract class EditorZone<T> : MonoBehaviour where T : MonoBehaviour
{
    public List<string> activateTags = new() { "Player" };
    [Min(0)]
    [Tooltip("The number of times the player can walk in before the modifier is not applied anymore (0 = infinite)")]
    public int numberOfAllowedActivations = 1;
    [Min(0)]
    [Tooltip("The duration of the camera modifier in seconds (0 = until collision ends)")]
    public float duration = 0;
    [Min(0)]
    [Tooltip("The cooldown between activations in seconds")]
    public float cooldown = 0;
    [Tooltip("Determins whether player movement should be frozen OnActivate and unfrozen after cooldown ends")]
    public List<string> freezeTags = new();
    public bool deactivateOnExit = true;

    [Header("Gizmo Settings")]
    public bool showZone = true;
    public Color zoneColor = Color.green;

    public UnityEvent<GameObject> OnActivate = new();
    public UnityEvent OnDeactivate = new();
    public UnityEvent OnCooldownReset = new();

    public float activationSeconds = 1f;

    protected float currentCooldown = 0;
    protected float currentDuration = 0;
    protected int activations = 0;
    protected Collider2D zoneCollider;

    protected List<string> inZone = new();
    private List<CharacterStateMachine> frozenCharacters = new();

    #region Lifecycle Events
    protected virtual void Start() { }
    protected virtual void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (!zoneCollider.isTrigger)
        {
            Debug.LogWarning($"Zone collider on {gameObject.name} is not set to 'Trigger'. Setting it automatically.");
            zoneCollider.isTrigger = true;
        }

        OnDeactivate.AddListener(() =>
        {
            frozenCharacters.ForEach((sm) =>
            {
                sm.Unfreeze();
            });
            frozenCharacters.Clear();
        });

        OnActivate.AddListener((go) =>
        {
            activations++;

            // Set and reset cooldown
            StartCoroutine(Cooldown());
            StartCoroutine(Duration());
            IEnumerator Cooldown()
            {
                if (cooldown != 0)
                {
                    currentCooldown = cooldown;
                    while (currentCooldown > 0)
                    {
                        currentCooldown -= Time.deltaTime;
                        yield return null;
                    }

                    OnCooldownReset.Invoke();
                }
            }
            IEnumerator Duration()
            {
                currentDuration = 0;

                while (currentDuration < duration)
                {
                    currentDuration += Time.deltaTime;
                    yield return null;
                }

                OnDeactivate.Invoke();
            }

            if (freezeTags.Contains(tag) && go.TryGetComponent(out CharacterStateMachine sm))
            {
                sm.Freeze();
                frozenCharacters.Add(sm);
            }
        });

    }

    // Set the camera type to the modifier type when the player enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (numberOfAllowedActivations > 0 && activations >= numberOfAllowedActivations || other.isTrigger) return;
        var tag = other.transform.root.tag;
        inZone.Add(tag);
        if (!activateTags.Contains(tag) || currentCooldown > 0 || inZone.Count > 1) return; // Check if the tag is in the list of allowed tags
        OnActivate.Invoke(other.transform.root.gameObject);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        var tag = other.transform.root.tag;
        inZone.Remove(tag);
        if (!activateTags.Contains(tag) || other.isTrigger || !deactivateOnExit || inZone.Count > 0) return; // Check if the tag is in the list of allowed tags

        OnDeactivate.Invoke();
    }

    #endregion

    #region Gizmos
    // Draw the zone in the editor
    public virtual void OnDrawGizmos()
    {
        if (!showZone) return;

        // Need to get component seperately here because Awake() is not called in editor mode
        if (!TryGetComponent(out Collider2D collider))
            return;

        // Get the collider's bounds
        Bounds bounds = collider.bounds;

        // Draw the semi-transparent fill
        Gizmos.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.1f);
        Gizmos.DrawCube(bounds.center, bounds.size);
        // Draw the border
        Gizmos.color = zoneColor;
        Gizmos.DrawWireCube(bounds.center, bounds.size);


    }
    #endregion
}
