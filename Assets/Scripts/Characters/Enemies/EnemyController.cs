using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(EnemyStateMachine))]
[RequireComponent(typeof(Seeker))]
public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public EnemyStateMachine stateMachine;

    public event Action OnFlip = delegate { };

    public bool IsFacingRight { get; private set; } = true;

    public List<Transform> patrolPoints = new();
    private int currentPatrolPointIndex = 0;
    private Vector2 patrolPointCenter;
    public int maxPlayerFollowDistance = 20;
    public int detectionDistance = 10;
    public bool forceFollow = false;
    public int forceFollowThreshold = 40;
    private bool followModeEngaged = false;

    private void Start()
    {
        stateMachine = GetComponent<EnemyStateMachine>();

        if (patrolPoints.Count > 0)
        {
            patrolPointCenter = Vector2.zero;
            foreach (var point in patrolPoints)
            {
                patrolPointCenter += (Vector2)point.position;
            }
            patrolPointCenter /= patrolPoints.Count;
        }
        else
        {
            patrolPointCenter = stateMachine.rb.position;
        }
    }
    private void Update()
    {
        if (patrolPoints.Count == 0)
        {
            patrolPointCenter = stateMachine.rb.position;
        }

        var hit = Physics2D.OverlapCircle(stateMachine.rb.position, detectionDistance, LayerMask.GetMask("Player"));
        var isFollowingPlayer = stateMachine.target && stateMachine.target.CompareTag("Player");
        if (isFollowingPlayer)
        {
            var distanceFromOrigin = Vector2.Distance(stateMachine.target.position, patrolPointCenter);
            if (distanceFromOrigin > maxPlayerFollowDistance)
            {
                stateMachine.target = null;
            }
        }
        else if (hit)
        {
            stateMachine.target = hit.transform.root;
        }
        else if (patrolPoints.Count > 0)
        {
            var distanceToPatrolPoint = Vector2.Distance(patrolPoints[currentPatrolPointIndex].position, stateMachine.rb.position);
            if (distanceToPatrolPoint < 0.5f)
            {
                currentPatrolPointIndex++;
                if (currentPatrolPointIndex >= patrolPoints.Count)
                {
                    currentPatrolPointIndex = 0;
                }
                stateMachine.target = patrolPoints[currentPatrolPointIndex];
            }
        }


        if (stateMachine.IsActive) FlipCheck();
    }
    private void FixedUpdate()
    {
        HandleGravity();
        HandleFall();
        HandleForceFollow();

        if (stateMachine.IsActive)
        {
            HandleMovement();
            HandleJump();
        }

    }
    private void HandleForceFollow()
    {
        if (!stateMachine.target || stateMachine.path == null)
            return;

        var distance = stateMachine.path.GetTotalLength();
        if (forceFollow)
        {
            if (distance > forceFollowThreshold)
            {
                Debug.Log("Force follow");
                stateMachine.rb.velocity = stateMachine.pathDir * stateMachine.config.MaxWalkSpeed;
                stateMachine.rb.isKinematic = true;
                followModeEngaged = true;
            }
        }

        if (followModeEngaged && distance <= forceFollowThreshold)
        {
            stateMachine.rb.isKinematic = false;
            followModeEngaged = false;
        }
    }

    private void HandleGravity()
    {
        if (!stateMachine.ground.connected)
        {
            stateMachine.rb.AddForce(Physics2D.gravity * stateMachine.config.GravityMultiplier);
        }
    }

    private void FlipCheck()
    {
        if (stateMachine.pathDir.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
            transform.rotation = Quaternion.Euler(0f, 0, 0f);
            OnFlip.Invoke();
        }
        else if (stateMachine.pathDir.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            OnFlip.Invoke();
        }
    }

    private void HandleMovement()
    {

        if (stateMachine.IsStateActive(EEnemyState.Moving, EEnemyState.Accelerating))
            return;

        stateMachine.EnterState(EEnemyState.Accelerating);
        if (stateMachine.IsStateActive(EEnemyState.Moving, EEnemyState.Accelerating))
        {
            stateMachine.EnterState(EEnemyState.Decelerating);
        }


    }
    private void HandleJump()
    {
        if (!stateMachine.ground.connected || stateMachine.IsStateActive(EEnemyState.Jumping))
            return;

        Rigidbody2D rb = stateMachine.rb;
        EnemyMovementConfig config = stateMachine.config;
        Vector2 forward = new(Mathf.Sign(stateMachine.pathDir.x), 0);

        if (stateMachine.pathAngle <= 40)
        {
            stateMachine.EnterState(EEnemyState.Jumping);
            return;
        }

        RaycastHit2D wallHit = Physics2D.Raycast(
            stateMachine.groundCheckCollider.transform.position,
            forward,
            config.jumpDetectionDistance,
            config.GroundLayer
        );
        bool isWall = !!wallHit.collider && wallHit.normal.y < 0.3f; // ! swap 0.3f with config.WallAngleThreshold

        if (isWall)
        {
            stateMachine.EnterState(EEnemyState.Jumping);
            return;
        }

        var rayOrigin = (Vector2)stateMachine.rb.position + forward * config.jumpDetectionDistance;

        RaycastHit2D ledgeHit = Physics2D.Raycast(
            rayOrigin,
            -rb.transform.up,
            config.LedgeDepthThreshold,
            config.GroundLayer
        );
        Debug.DrawRay(rayOrigin, -rb.transform.up * config.LedgeDepthThreshold, Color.blue);

        bool isLedge = !ledgeHit.collider;
        bool wantsToGoDown = stateMachine.pathAngle >= 100;
        if (!wantsToGoDown && isLedge)
        {
            stateMachine.EnterState(EEnemyState.Jumping);
        }

    }
    private void HandleFall()
    {
        // TODO: might have to replace with or add a check for the fall state being active
        if (stateMachine.rb.velocity.y < 0 && !stateMachine.ground.connected && !stateMachine.IsStateActive(EEnemyState.Jumping, EEnemyState.Falling))
        {
            stateMachine.EnterState(EEnemyState.Falling);
        }
    }
}
