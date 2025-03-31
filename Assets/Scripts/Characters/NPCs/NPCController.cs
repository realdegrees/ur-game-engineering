using System;
using System.Collections.Generic;
using System.Collections;
using Pathfinding;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(NPCStateMachine))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class NPCController : MonoBehaviour
{
    [HideInInspector]
    public NPCStateMachine stateMachine;

    public event Action OnFlip = delegate { };

    public bool IsFacingRight { get; private set; } = true;

    public List<Transform> patrolPoints = new();

    [SerializeField] int damage;
    public AudioClip[] attackSounds;
    public float attackCooldown = 1.5f;
    public bool isRanged;

    [SerializeField] GameObject projectile;

    private AudioSource audioSource;
    private Animator animator;
    private int currentPatrolPointIndex = 0;
    private Vector2 patrolPointCenter;
    public List<string> attacksTags = new() { };
    public List<string> followsTags = new() { };
    public int maxFollowRangeFromOrigin = 0;
    public bool forceFollow = false;
    public int forceFollowThreshold = 4;
    private bool forceFollowActive = false;
    private DateTime lastAttack = DateTime.Now;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        stateMachine = GetComponent<NPCStateMachine>();
        animator = GetComponent<Animator>();

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

        stateMachine.OnStateEnter += (state) =>
        {
            if (state.state == ENPCState.Jumping)
            {
                animator.SetTrigger("Jump");
            }
        };
    }
    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(stateMachine.rb.velocity.x));
        animator.SetBool("Grounded", stateMachine.ground.connected);
        if (patrolPoints.Count == 0)
        {
            patrolPointCenter = stateMachine.rb.position;
        }

        var hit = Physics2D.OverlapCircleAll(stateMachine.rb.position, stateMachine.Config.FollowDistance);
        var target = hit.FirstOrDefault(h => followsTags.Any((t) => h.transform.root.CompareTag(t)));
        if (stateMachine.Target && maxFollowRangeFromOrigin > 0)
        {
            var distanceFromOrigin = Vector2.Distance(stateMachine.Target.position, patrolPointCenter);
            if (distanceFromOrigin > maxFollowRangeFromOrigin)
            {
                stateMachine.SetTarget(null);
            }
        }
        else if (target)
        {
            stateMachine.SetTarget(target.transform.root);
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
                stateMachine.SetTarget(patrolPoints[currentPatrolPointIndex]);
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
        if ((DateTime.Now - lastAttack).Seconds > attackCooldown && stateMachine.Target)
        {
            HandleAttack();
        }
    }

    private void HandleAttack()
    {
        var targetStats = stateMachine.Target.GetComponent<CharacterStats>();
        if (!targetStats || targetStats.GetHealth() <= 0)
            return;

        var losCheck = Physics2D.Linecast(transform.position, stateMachine.Target.transform.position, LayerMask.GetMask("Ground"));
        if (losCheck.collider != null || Vector2.Distance(stateMachine.Target.position, stateMachine.rb.position) > stateMachine.Config.ResumeDistance)
            return;

        DoAttack(targetStats);
    }

    private void DoAttack(CharacterStats targetStats)
    {
        animator.SetTrigger("Attack");
        if (isRanged)
        {
            var projectileGo = Instantiate(projectile, transform.position + transform.forward * 0.5f, Quaternion.identity);
            var projectileScript = projectileGo.GetComponent<ProjectileScript>();
            projectileScript.damage = damage;
            projectileScript.ignoresTags.Add(transform.root.tag);
            projectileScript.Init(stateMachine.Target.position - transform.position);
        }
        else
        {
            targetStats.TakeDamage(damage);

        }
        PlayRandomAttackSound();
        lastAttack = DateTime.Now;
    }

    private void PlayRandomAttackSound()
    {
        if (attackSounds.Length > 0)
        {
            AudioClip selectedSound = attackSounds[UnityEngine.Random.Range(0, attackSounds.Length)];
            audioSource.PlayOneShot(selectedSound);
        }
    }

    private void HandleForceFollow()
    {
        if (!stateMachine.Target || stateMachine.path == null)
            return;

        var distance = stateMachine.path.GetTotalLength();
        if (forceFollow)
        {
            if (distance > forceFollowThreshold)
            {
                Debug.Log("Force follow");
                stateMachine.rb.velocity = stateMachine.pathDir * stateMachine.Config.MaxWalkSpeed;
                stateMachine.rb.isKinematic = true;
                forceFollowActive = true;
            }
        }

        if (forceFollowActive && distance <= forceFollowThreshold)
        {
            stateMachine.rb.isKinematic = false;
            forceFollowActive = false;
        }
    }

    private void HandleGravity()
    {
        if (!stateMachine.ground.connected)
        {
            stateMachine.rb.AddForce(Physics2D.gravity * stateMachine.Config.GravityMultiplier);
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

        if (stateMachine.IsStateActive(ENPCState.Moving, ENPCState.Accelerating))
            return;

        stateMachine.EnterState(ENPCState.Accelerating);
        if (stateMachine.IsStateActive(ENPCState.Moving, ENPCState.Accelerating))
        {
            stateMachine.EnterState(ENPCState.Decelerating);
        }


    }
    private void HandleJump()
    {
        if (!stateMachine.ground.connected || stateMachine.IsStateActive(ENPCState.Jumping) || !stateMachine.IsActive)
            return;

        Rigidbody2D rb = stateMachine.rb;
        NPCMovementConfig config = stateMachine.Config;
        Vector2 forward = new(Mathf.Sign(stateMachine.pathDir.x), 0);

        if (stateMachine.pathAngle <= 40)
        {
            stateMachine.EnterState(ENPCState.Jumping);
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
            stateMachine.EnterState(ENPCState.Jumping);
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
            stateMachine.EnterState(ENPCState.Jumping);
        }

    }
    private void HandleFall()
    {
        // TODO: might have to replace with or add a check for the fall state being active
        if (stateMachine.rb.velocity.y < 0 && !stateMachine.ground.connected && !stateMachine.IsStateActive(ENPCState.Jumping, ENPCState.Falling))
        {
            stateMachine.EnterState(ENPCState.Falling);
        }
    }
}
