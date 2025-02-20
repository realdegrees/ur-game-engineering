using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyStateMachine : StateMachine<EEnemyState, EnemyMovementConfig>
{
    // region Pathfinding,
    [HideInInspector] public Seeker seeker;
    [HideInInspector] public Path path;
    [HideInInspector] public Transform target;
    [HideInInspector] public int currentWaypoint = 0;
    // endregion

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
        TryGetComponent(out seeker);
        Debug.Log(rb);
        States.ForEach(state => ((EnemyState)state).SetRigidbody(rb));

        StartCoroutine(UpdatePath());
    }
    IEnumerator UpdatePath() // Called in Awake
    {
        while (true)
        {
            if (seeker.IsDone() && target != null)
            {
                Debug.Log("Calulating new path");
                seeker.StartPath(rb.position, target.position, p =>
                {
                    if (!p.error)
                    {
                        path = p;
                        currentWaypoint = 0;
                    }
                });
            }
            yield return new WaitForSeconds(config.pathUpdateDelta);
        }
    }
    protected override void Update()
    {
        // var activeStatesLog = new List<string>();
        // activeStates.ForEach(s => activeStatesLog.Add(s.state.ToString()));
        // Debug.Log(string.Join(", ", activeStatesLog));

        GroundCheck();
        CeilingCheck();
        if (path != null) HandlePathTraversion();

        // This needs to be called as all states are updated in the base class
        base.Update();
    }
    private void HandlePathTraversion()
    {
        if (currentWaypoint >= path.vectorPath.Count)
        {
            path = null;
            return;
        }
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < config.nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
    #region Generic State Checks

    private void GroundCheck()
    {
        ground.hit = Physics2D.CapsuleCast(groundCheckCollider.bounds.center, groundCheckCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, -transform.up, config.BottomRange, config.GroundLayer);
        ground.collider = ground.hit.collider;
        ground.connected = ground.collider != null;
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