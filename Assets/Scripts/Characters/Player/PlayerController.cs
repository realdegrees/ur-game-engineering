using System;
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

    public bool IsFacingRight { get; private set; } = true;

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
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateMachine.rb.constraints == RigidbodyConstraints2D.FreezePosition || stateInfo.IsName("Attack_1") || stateInfo.IsName("Attack_2") || stateInfo.IsName("Attack_3"))
                return;

            // get enemies in range
            // deal damage to enemies
            // trigger animation
            var hit = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Enemy"));
            if (hit)
            {
                hit.GetComponent<CharacterStats>().TakeDamage(playerStats.GetDamage());
            }
            int attackNumber = UnityEngine.Random.Range(1, 4); // Generates a random number between 1 and 3 (inclusive)
            animator.SetTrigger($"Attack_{attackNumber}");
        };

        // TODO wherever the player attack method is called, also trigger the animation parameter "Attack"
        stateMachine.OnStateEnter += (state) =>
        {
            if (state.state == ECharacterState.Jumping)
            {
                animator.SetTrigger("Jump");
            }
            else if (state.state == ECharacterState.Falling)
            {
                animator.SetTrigger("Fall");
            }
        };
    }
    private void Update()
    {
        FlipCheck();
    }
    private void FixedUpdate()
    {
        animator.SetFloat("Speed", Mathf.Abs(stateMachine.rb.velocity.x));
        animator.SetFloat("YVelocity", Mathf.Abs(stateMachine.rb.velocity.y));
        animator.SetBool("Grounded", stateMachine.ground.connected);
        HandleGravity();
        HandleMovement();
        HandleFall();
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
        var isInCoyoteWindow = Time.time - stateMachine.ground.lastConnected < stateMachine.config.CoyoteTime;
        var canJump = stateMachine.jumpsSinceGrounded < stateMachine.config.NumberOfJumpsAllowed;

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