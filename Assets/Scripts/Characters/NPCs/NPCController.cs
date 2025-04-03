using System;
using System.Collections.Generic;
using System.Collections;
using Pathfinding;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(NPCStateMachine))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterStats))]
public class NPCController : MonoBehaviour
{
    [HideInInspector]
    public NPCStateMachine stateMachine;

    public event Action OnFlip = delegate { };

    public bool IsFacingRight { get; private set; } = true;

    public List<Transform> patrolPoints = new();

    public AudioClip[] attackSounds;
    public float attackCooldown = 1.5f;
    public bool isRanged;

    [SerializeField] GameObject projectile;

    private AudioSource audioSource;
    protected Animator animator;
    private CharacterStats characterStats;
    private int currentPatrolPointIndex = 0;
    private Vector2 patrolPointCenter;
    public Transform fallbackFollowTarget = null;
    public List<string> followsTags = new() { };
    public List<string> attacksTags = new() { };
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
        characterStats = GetComponent<CharacterStats>();

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
            if (state.state == ECharacterState.Jumping)
            {
                if (animator.parameters.FirstOrDefault((p) => p.name == "Jump") != null) animator.SetTrigger("Jump");
            }
        };
    }
    protected virtual void Update()
    {
        if (animator.parameters.FirstOrDefault((p) => p.name == "Speed") != null) animator.SetFloat("Speed", Mathf.Abs(stateMachine.rb.velocity.x));
        if (animator.parameters.FirstOrDefault((p) => p.name == "Grounded") != null) animator.SetBool("Grounded", stateMachine.ground.connected);
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
                stateMachine.SetTarget(fallbackFollowTarget);
            }
        }
        else if (target)
        {
            var hasLos = Physics2D.Linecast(stateMachine.rb.position, target.transform.position, LayerMask.GetMask("Ground")).collider == null;
            if (hasLos) stateMachine.SetTarget(target.transform.root);
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

        FlipCheck();
    }
    private void FixedUpdate()
    {
        HandleGravity();
        HandleFall();

        if (stateMachine.IsActive)
        {
            HandleMovement();
            HandleJump();
        }
        if ((DateTime.Now - lastAttack).Seconds > attackCooldown && stateMachine.Target)
        {
            HandleAttack();
        }
        HandleForceFollow();
    }

    private void HandleAttack()
    {
        if (!attacksTags.Any((t) => stateMachine.Target.CompareTag(t))) return;
        var targetStats = stateMachine.Target.GetComponent<CharacterStats>();
        if (!targetStats || targetStats.GetHealth() <= 0)
            return;

        var losCheck = Physics2D.Linecast(transform.position, stateMachine.Target.transform.position, LayerMask.GetMask("Ground"));
        if (losCheck.collider != null || Vector2.Distance(stateMachine.Target.position, stateMachine.rb.position) > stateMachine.Config.ResumeDistance || stateMachine.rb.constraints == RigidbodyConstraints2D.FreezePosition)
            return;

        DoAttack(targetStats);
    }

    private void DoAttack(CharacterStats targetStats)
    {
        if (animator.parameters.FirstOrDefault((p) => p.name == "Attack") != null) animator.SetTrigger("Attack");
        if (isRanged)
        {
            var projectileGo = Instantiate(projectile, transform.position + transform.forward * 0.5f, Quaternion.identity);
            var projectileScript = projectileGo.GetComponent<ProjectileScript>();
            projectileScript.damage = characterStats.damage;
            projectileScript.ignoresTags.Add(transform.root.tag);
            projectileScript.Init(stateMachine.Target.position - transform.position);
        }
        else
        {
            targetStats.TakeDamage(characterStats.damage);

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
        if (stateMachine.IsActive)
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
        else if (stateMachine.Target)
        {
            if (stateMachine.Target.position.x > transform.position.x && !IsFacingRight)
            {
                IsFacingRight = true;
                transform.rotation = Quaternion.Euler(0f, 0, 0f);
                OnFlip.Invoke();
            }
            else if (stateMachine.Target.position.x < transform.position.x && IsFacingRight)
            {
                IsFacingRight = false;
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                OnFlip.Invoke();
            }
        }
    }

    private void HandleMovement()
    {

        if (stateMachine.IsStateActive(ECharacterState.Moving, ECharacterState.Accelerating))
            return;

        stateMachine.EnterState(ECharacterState.Accelerating);
        if (stateMachine.IsStateActive(ECharacterState.Moving, ECharacterState.Accelerating))
        {
            stateMachine.EnterState(ECharacterState.Decelerating);
        }


    }
    private void HandleJump()
    {
        if (!stateMachine.ground.connected || stateMachine.IsStateActive(ECharacterState.Jumping) || !stateMachine.IsActive || !stateMachine.Target)
            return;

        Rigidbody2D rb = stateMachine.rb;
        NPCMovementConfig config = stateMachine.Config;
        Vector2 forward = new(Mathf.Sign(stateMachine.pathDir.x), 0);
        var targetStateMachine = stateMachine.Target.GetComponent<CharacterStateMachine>();
        if (stateMachine.pathAngle <= 40 && (!targetStateMachine || !targetStateMachine.IsStateActive(ECharacterState.Jumping)))
        {
            stateMachine.EnterState(ECharacterState.Jumping);
            return;
        }

        RaycastHit2D wallHit = Physics2D.Raycast(
            stateMachine.groundCheckCollider.bounds.center,
            forward,
            config.jumpDetectionDistance,
            config.GroundLayer
        );
        bool isWall = !!wallHit.collider; // ! swap 0.3f with config.WallAngleThreshold

        if (isWall)
        {
            stateMachine.EnterState(ECharacterState.Jumping);
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
            stateMachine.EnterState(ECharacterState.Jumping);
        }

    }
    private void HandleFall()
    {
        // TODO: might have to replace with or add a check for the fall state being active
        if (stateMachine.rb.velocity.y < 0 && !stateMachine.ground.connected && !stateMachine.IsStateActive(ECharacterState.Jumping, ECharacterState.Falling))
        {
            stateMachine.EnterState(ECharacterState.Falling);
        }
    }
}
