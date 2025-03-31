using System;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterStateMachine))]
public class PlayerController : Manager<PlayerController>
{
    private Animator animator;
    [HideInInspector]
    public CharacterStateMachine stateMachine;

    private PlayerStats playerStats;

    public event Action OnFlip = delegate { };
    public float attackRange = 1f;
    public bool IsFacingRight { get; private set; } = true;
    public float attackCooldown = 1f;
    private DateTime lastAttack = DateTime.Now;

    protected void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            stateMachine.rb.transform.position = Vector3.zero;
        };

        TryGetComponent(out animator);
        TryGetComponent(out stateMachine);
        TryGetComponent(out playerStats);
        InputManager.Instance.OnJumpPressed += HandleJump;
        InputManager.Instance.OnAttackPressed += () =>
        {
            if (stateMachine.rb.constraints == RigidbodyConstraints2D.FreezePosition || (DateTime.Now - lastAttack).TotalMilliseconds <= attackCooldown * 1000)
                return;

            // get enemies in range
            // deal damage to enemies
            // trigger animation
            var hit = Physics2D.OverlapCircleAll(transform.position, attackRange);
            var hostileHitObj = hit.FirstOrDefault((h) => h.transform.root.CompareTag("Hostile"));
            var enemy = hostileHitObj != null ? hostileHitObj.transform.root : null;
            if (enemy)
            {
                var losCheck = Physics2D.Linecast(transform.position, enemy.transform.position, LayerMask.GetMask("Ground"));
                if (losCheck.collider != null)
                    return;

                if (enemy.CompareTag("Hostile") && enemy.TryGetComponent(out CharacterStats stats))
                {
                    stats.TakeDamage(playerStats.GetDamage());
                }
            }
            animator.SetTrigger("Attack");
            lastAttack = DateTime.Now;
        };

        // TODO wherever the player attack method is called, also trigger the animation parameter "Attack"
        stateMachine.OnStateEnter += (state) =>
        {
            if (state.state == ECharacterState.Jumping)
            {
                animator.SetTrigger("Jump");
            }
        };
    }
    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(stateMachine.rb.velocity.x));
        animator.SetBool("Grounded", stateMachine.ground.connected);
        FlipCheck();
    }
    private void FixedUpdate()
    {
        HandleGravity();
        HandleMovement();
        HandleFall();
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
        if (InputManager.Instance.Movement > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
            transform.rotation = Quaternion.Euler(0f, 0, 0f);
            OnFlip.Invoke();
        }
        else if (InputManager.Instance.Movement < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            OnFlip.Invoke();
        }
    }

    private void HandleMovement()
    {
        if (InputManager.Instance.Movement == 0)
            return;

        if (stateMachine.IsStateActive(ECharacterState.Moving, ECharacterState.Accelerating, ECharacterState.Decelerating))
            return;


        stateMachine.EnterState(ECharacterState.Accelerating);
    }
    private void HandleJump()
    {
        // var isGroundedOrFalling = stateMachine.IsStateActive(ECharacterState.Falling, ECharacterState.JumpApex) || stateMachine.ground.connected;
        var isInCoyoteWindow = Time.time - stateMachine.ground.lastConnected < stateMachine.Config.CoyoteTime;
        var canJump = stateMachine.jumpsSinceGrounded < stateMachine.Config.NumberOfJumpsAllowed;

        if (isInCoyoteWindow || canJump)
        {
            stateMachine.EnterState(ECharacterState.Jumping);
        }
    }
    private void HandleFall()
    {
        // TODO: might have to replace with or add a check for the fall state being active
        if (stateMachine.rb.velocity.y < 0 && !stateMachine.ground.connected && !stateMachine.IsStateActive(ECharacterState.Jumping, ECharacterState.JumpApex, ECharacterState.Falling))
        {
            // Removes a jump if they player starts falling without jumping first
            stateMachine.jumpsSinceGrounded++;
            stateMachine.EnterState(ECharacterState.Falling);
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        InputManager.Instance.OnJumpPressed -= HandleJump;
    }
}