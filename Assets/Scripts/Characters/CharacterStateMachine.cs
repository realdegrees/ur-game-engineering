using System;
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
    public float lastConnected;
    public bool connectedOnThisFrame;
    public bool connected;
    public float angle;
    public Vector2 perpendicular;
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

    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public int jumpsSinceGrounded = 0;

    // public bool IsFalling() => Vector2.Dot(rb.velocity, Physics2D.gravity.normalized) < 0;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out rb);
        Debug.Log(rb);
        States.ForEach(state => ((CharacterState)state).SetRigidbody(rb));
    }

    protected override void Update()
    {
        // var activeStatesLog = new List<string>();
        // activeStates.ForEach(s => activeStatesLog.Add(s.state.ToString()));
        // Debug.Log(string.Join(", ", activeStatesLog));

        GroundCheck();
        CeilingCheck();

        // This needs to be called as all states are updated in the base class
        base.Update();
    }

    #region Generic State Checks
    private void GroundCheck()
    {
        var prev = ground.connected;
        ground.hit = Physics2D.CapsuleCast(groundCheckCollider.bounds.center, groundCheckCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, -transform.up, config.BottomRange, config.GroundLayer);
        ground.collider = ground.hit.collider;
        ground.connected = ground.collider != null;
        ground.connectedOnThisFrame = false;
        ground.relativeVelocity = ground.collider && ground.collider.attachedRigidbody ? rb.velocity - ground.collider.attachedRigidbody.velocity : rb.velocity;
        ground.angle = ground.connected ? Vector2.Angle(transform.up, ground.hit.normal) : 0;
        ground.angleType = ground.angle > .5f ? (Vector2.Dot(ground.hit.normal, transform.up) > 0 ? EGroundAngle.Down : EGroundAngle.Up) : EGroundAngle.Flat;
        ground.perpendicular = ground.connected ? Vector2.Perpendicular(ground.hit.normal).normalized : Vector2.left;

        if (!ground.connected)
        {
            ground.hit.normal = transform.up;
        }
        else
        {
            if (!prev)
            {
                ground.connectedOnThisFrame = true;
            }
            ground.lastConnected = Time.time;
        }
    }

    private void CeilingCheck()
    {
        ceiling.hit = Physics2D.CapsuleCast(ceilingCheckCollider.bounds.center, ceilingCheckCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, transform.up, config.TopRange, config.GroundLayer);
        ceiling.collider = ceiling.hit.collider;
        ceiling.angle = Vector2.Angle(transform.up, ceiling.hit.normal);
        ceiling.connected = ceiling.collider != null && ceiling.angle <= config.CeilingAngleThreshold;
        ceiling.perpendicular = ceiling.connected ? Vector2.Perpendicular(ceiling.hit.normal).normalized : Vector2.left;

        if (!ceiling.connected)
        {
            ceiling.hit.normal = -transform.up;
        }
        else
        {
            ceiling.lastConnected = Time.time;
        }
    }

    #endregion
}