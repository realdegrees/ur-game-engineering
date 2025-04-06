using System;
using System.Collections;
using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class NPCStateMachine : StateMachine<ECharacterState, NPCMovementConfig>
{
    public bool canFly = false;
    // region Pathfinding,
    [HideInInspector] public Seeker seeker;
    [HideInInspector] public Path path;
    public Transform Target { get; private set; } = null;
    [HideInInspector] public Vector2 pathDir = Vector2.down;
    [HideInInspector] public float pathAngle = 0;
    [HideInInspector] public int currentWaypoint = 0;
    // endregion

    public Collider2D groundCheckCollider;
    // [SerializeField] private Collider2D bodyCollider;
    public Collider2D ceilingCheckCollider;
    public Collider2D bodyCollider;
    // [SerializeField] private Collider2D interactionCollider;
    public PlayerBoundaryElement ground = new();
    public PlayerBoundaryElement ceiling = new();
    private Action OnLand = delegate { };
    public Transform fallbackFollowTarget = null;

    public bool IsFacingRight { get; private set; } = true;

    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public int jumpsSinceGrounded = 0;

    public bool IsActive { get; private set; } = false;
    public event Action<Transform> OnTargetChanged;

    private void NotifyTargetChanged(Transform newTarget)
    {
        OnTargetChanged?.Invoke(newTarget);
    }

    public void SetTarget(Transform newTarget)
    {
        Target = newTarget;
        path = Target != null ? path : null;
        IsActive = Target != null;
        NotifyTargetChanged(newTarget);
    }

    private void DoFreeze()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        OnLand -= DoFreeze;
    }
    public void Freeze()
    {
        if (canFly)
        {
            DoFreeze();
            return;
        }
        if (ground.connected) DoFreeze();
        else OnLand += DoFreeze;
    }
    public void Unfreeze()
    {
        OnLand -= DoFreeze;
        if (rb)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    // public bool IsFalling() => Vector2.Dot(rb.velocity, Physics2D.gravity.normalized) < 0;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out rb);
        TryGetComponent(out seeker);
        States.ForEach(state => ((NPCState)state).SetRigidbody(rb));
        OnTargetChanged += (target) =>
        {
            if (target == null && fallbackFollowTarget)
            {
                SetTarget(fallbackFollowTarget);
            }
        };
        StartCoroutine(UpdatePath());
    }
    private void Start()
    {
        if (Target == null)
        {
            SetTarget(null);
        }
    }
    IEnumerator UpdatePath() // Called in Awake
    {
        // Vector2 prevPos = rb.position;
        while (true)
        {
            if (!rb.isKinematic && (Target == null || !seeker.IsDone() || !IsActive))
            {
                yield return new WaitForSeconds(Config.pathUpdateDelta);
                continue;
            }
            // float targetDelta = path == null ? Vector2.Distance(target.position, rb.position) : Vector2.Distance(target.position, path.vectorPath.Last());
            // if (ground.connectedOnThisFrame || targetDelta > config.FollowDistance)

            seeker.StartPath(groundCheckCollider.bounds.min, Target.position, p =>
            {
                if (!p.error)
                {
                    path = p;
                    currentWaypoint = Math.Min(currentWaypoint, 0);
                }
            });

            yield return new WaitForSeconds(Config.pathUpdateDelta);
        }
    }
    protected override void Update()
    {
        // var activeStatesLog = new List<string>();
        // activeStates.ForEach(s => activeStatesLog.Add(s.state.ToString()));
        // Debug.Log(string.Join(", ", activeStatesLog));

        GroundCheck();
        CeilingCheck();
        CalcPathDir();

        if (Target != null && Target.TryGetComponent(out CharacterStats stats))
        {
            if (stats.GetHealth() <= 0)
            {
                SetTarget(fallbackFollowTarget);
            }
        }
        if (Target != null && path != null)
        {
            TryGetComponent(out NPCController npcController);
            var shouldMove = Vector2.Distance(Target.position, rb.transform.position) > (npcController != null && npcController.isRanged ? Config.ResumeDistance : Config.FollowDistance) || Physics2D.Linecast(ceilingCheckCollider.bounds.center, Target.position, Config.GroundLayer).collider != null;
            if (shouldMove) IsActive = true;
        }

        if (IsActive && path != null) HandlePathTraversion();

        // This needs to be called as all states are updated in the base class
        base.Update();
    }
    private void CalcPathDir()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
            return;

        var earlyPathWeightingFactor = .5f;
        //int waypointsToConsider = Mathf.Min(Config.WayPointLookAhead, path.vectorPath.Count - currentWaypoint - 1);
        int waypointsToConsider = Mathf.Min(3, path.vectorPath.Count - currentWaypoint - 1);
        pathDir = Vector2.zero;
        pathAngle = 0;

        for (int i = 0; i < waypointsToConsider; i++)
        {

            var dir = ((Vector2)path.vectorPath[currentWaypoint + i + 1] - (Vector2)path.vectorPath[currentWaypoint]) * (1 + earlyPathWeightingFactor * (waypointsToConsider - i) / waypointsToConsider);
            pathDir += dir;
            pathAngle += Vector2.SignedAngle(Vector2.up, dir);
        }

        pathDir /= waypointsToConsider;
        pathAngle /= waypointsToConsider;
        pathAngle = Mathf.Abs(pathAngle);
        Debug.DrawRay(rb.position, pathDir * 2, Color.red);
    }
    private void HandlePathTraversion()
    {
        //if (IsStateActive(ECharacterState.Jumping)) return; // ! Might need to delete this

        var shouldStop = Vector2.Distance(rb.position, Target.transform.position) < Config.FollowDistance && Physics2D.Linecast(ceilingCheckCollider.bounds.center, Target.position, Config.GroundLayer).collider == null;
        if ((shouldStop && ground.connected) || (currentWaypoint >= path.vectorPath.Count))
        {
            IsActive = false;
            return;
        }

        float distance;

        if (ground.connectedOnThisFrame)
        {
            float minDistance = float.MaxValue;
            for (int i = 0; i < path.vectorPath.Count; i++)
            {
                distance = Vector2.Distance(rb.position, path.vectorPath[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentWaypoint = i;
                }
            }
            return;
        }

        if (currentWaypoint < path.vectorPath.Count)
        {
            return;
        }
        //distance = Mathf.Abs(rb.position.x - path.vectorPath[currentWaypoint].x);
        distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        var horizontalDistance = Math.Abs(path.vectorPath[currentWaypoint].x - rb.position.x);
        var verticlalDirection = path.vectorPath[currentWaypoint].y - rb.position.y;
        if (distance <= 0.4f || (!ground.connected && horizontalDistance <= 1f && verticlalDirection < 0))
        {
            currentWaypoint++;
        }

        Debug.DrawLine(rb.position, path.vectorPath[currentWaypoint], Color.green);
        Debug.DrawRay(path.vectorPath[currentWaypoint], Vector3.up * 0.5f, Color.blue);
        Debug.DrawRay(path.vectorPath[currentWaypoint], Vector3.right * 0.5f, Color.blue);
        Debug.DrawRay(path.vectorPath[currentWaypoint], Vector3.down * 0.5f, Color.blue);
        Debug.DrawRay(path.vectorPath[currentWaypoint], Vector3.left * 0.5f, Color.blue);
    }
    #region Generic State Checks

    private void GroundCheck()
    {
        var prev = ground.connected;
        Physics2D.queriesHitTriggers = false;
        ground.hit = Physics2D.CapsuleCast(groundCheckCollider.bounds.center, groundCheckCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, -transform.up, Config.BottomRange, Config.GroundLayer);
        Physics2D.queriesHitTriggers = true;
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
                ground.connectedOnThisFrame = base.IsStateActive(ECharacterState.Falling);
                OnLand.Invoke();
            }
            ground.lastConnected = Time.time;
        }
    }

    private void CeilingCheck()
    {
        Physics2D.queriesHitTriggers = false;
        ceiling.hit = Physics2D.CapsuleCast(ceilingCheckCollider.bounds.center, ceilingCheckCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, transform.up, Config.TopRange, Config.GroundLayer);
        Physics2D.queriesHitTriggers = true;
        ceiling.collider = ceiling.hit.collider;
        ceiling.angle = Vector2.Angle(transform.up, ceiling.hit.normal);
        ceiling.connected = ceiling.collider != null && ceiling.angle <= Config.CeilingAngleThreshold;
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