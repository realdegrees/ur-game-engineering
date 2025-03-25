using System.Collections;
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
    public bool freezePlayer = false;

    [Header("Gizmo Settings")]
    public bool showZone = true;
    public Color zoneColor = Color.green;

    public UnityEvent OnActivate = new();
    public UnityEvent OnDeactivate = new();
    public UnityEvent OnCooldownReset = new();
    public UnityEvent OnDurationPassed = new();

    protected float currentCooldown = 0;
    protected float currentDuration = 0;
    protected int activations = 0;
    protected Collider2D zoneCollider;

    private Rigidbody2D rb;
    private CharacterStateMachine stateMachine;

    #region Lifecycle Events
    protected virtual void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (!zoneCollider.isTrigger)
        {
            Debug.LogWarning($"Zone collider on {gameObject.name} is not set to 'Trigger'. Setting it automatically.");
            zoneCollider.isTrigger = true;
        }
    }

    protected virtual void Start()
    {
        rb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        stateMachine = GameObject.FindWithTag("Player").GetComponent<CharacterStateMachine>();
        DialogueManagerInk.OnDialogueEnd += () =>
        {
            if (freezePlayer) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        };
    }

    // Set the camera type to the modifier type when the player enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (numberOfAllowedActivations > 0 && activations >= numberOfAllowedActivations) return;
        if (other.transform.root.TryGetComponent(out PlayerController controller)) // ! re-use snippet
        {
            if (other == controller.stateMachine.bodyCollider && currentCooldown <= 0)
            {
                // Adjust variables
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
                    OnDurationPassed.Invoke();
                }

                OnActivate.Invoke();
                if (freezePlayer && stateMachine)
                {
                    StartCoroutine(FreezePlayer());
                }
            }

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.TryGetComponent(out PlayerController controller)) // ! re-use snippet
        {
            if (other == controller.stateMachine.bodyCollider)
            {
                OnDeactivate.Invoke();
            }
        }
    }

    private IEnumerator FreezePlayer()
    {
        Debug.Log("Freezing player");
        while (!stateMachine.ground.connected)
        {
            yield return null;
        }
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
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
