using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// [SerializeField]
public struct PlayerBoundaryElement
{
    public RaycastHit2D hit;
    public Vector2 velocity;
    public Vector2 relativeVelocity;
    public Collider2D collider;
    public bool connected;
    public float angle;
    public EGroundAngle angleType;
}
public enum EGroundAngle { Flat, Down, Up }
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterStateMachine : StateMachine<ECharacterState, PlayerMovementConfig>
{
    public Collider2D groundCheckCollider;
    // [SerializeField] private Collider2D bodyCollider;
    public Collider2D ceilingCheckCollider;
    public Collider2D bodyCollider;
    // [SerializeField] private Collider2D interactionCollider;
    public PlayerBoundaryElement ground = new();
    public PlayerBoundaryElement ceiling = new();

    public bool IsFacingRight { get; private set; } = true;

    private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    [HideInInspector]
    public int jumpsSinceGrounded = 0;

    // public bool IsFalling() => Vector2.Dot(rb.velocity, Physics2D.gravity.normalized) < 0;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        States.ForEach(state => ((CharacterState)state).SetRigidbody(rb));
    }

    protected override void Update()
    {
        // var activeStatesLog = new List<string>();
        // activeStates.ForEach(s => activeStatesLog.Add(s.state.ToString()));
        // Debug.Log(string.Join(", ", activeStatesLog));

        FlipCheck();
        GroundCheck();
        CeilingCheck();

        // This needs to be called as all states are updated in the base class
        base.Update();
    }


    #region Generic State Checks
    private void FlipCheck()
    {
        if (ground.relativeVelocity.x > config.TurnThreshold && !IsFacingRight || ground.relativeVelocity.x < -config.TurnThreshold && IsFacingRight)
        {
            IsFacingRight = !IsFacingRight;
            transform.Rotate(0f, 180f, 0f);
            // OnFlip.Invoke(isFacingRight);
        }
    }
    private void GroundCheck()
    {
        ground.hit = Physics2D.CapsuleCast(groundCheckCollider.bounds.center, groundCheckCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, -transform.up, config.BottomRange, config.GroundLayer);
        ground.collider = ground.hit.collider;
        ground.connected = ground.collider != null;
        ground.relativeVelocity = ground.collider && ground.collider.attachedRigidbody ? rb.velocity - ground.collider.attachedRigidbody.velocity : rb.velocity;
        ground.angle = ground.connected ? Vector2.Angle(transform.up, ground.hit.normal) : 0;
        ground.angleType = ground.angle > .5f ? (Vector2.Dot(ground.hit.normal, transform.up) > 0 ? EGroundAngle.Down : EGroundAngle.Up) : EGroundAngle.Flat;
        if (!ground.connected)
        {
            ground.hit.normal = transform.up;
        }
    }

    private void CeilingCheck()
    {
        ceiling.hit = Physics2D.CapsuleCast(ceilingCheckCollider.bounds.center, ceilingCheckCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, transform.up, config.TopRange, config.GroundLayer);
        ceiling.collider = ceiling.hit.collider;
        ceiling.angle = Vector2.Angle(transform.up, ceiling.hit.normal);
        ceiling.connected = ceiling.collider != null && ceiling.angle <= config.CeilingAngleThreshold;
        if (!ceiling.connected)
        {
            ceiling.hit.normal = -transform.up;
        }
    }

    #endregion
}