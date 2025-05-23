using System;
using System.Linq;
using Assets.Scripts.Utility;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterStateMachine))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private Animator animator;
    [HideInInspector]
    public CharacterStateMachine stateMachine;
    private PlayerStats playerStats;
    public AudioClip[] attackSounds;
    private AudioSource audioSource;

    public event Action OnFlip = delegate { };
    public float attackRange = 1f;
    public bool IsFacingRight { get; private set; } = true;
    public float attackCooldown = 1f;
    private DateTime lastAttack = DateTime.Now;

    protected void Start()
    {
        TryGetComponent(out animator);
        TryGetComponent(out stateMachine);
        TryGetComponent(out playerStats);
        TryGetComponent(out audioSource);

        CameraManager.Instance.SetCameraType(CameraType.Follow, transform);
        InputManager.Instance.OnJumpPressed += HandleJump;
        InputManager.Instance.OnAttackPressed += HandleAttack;
        HandleAttack();

        // TODO wherever the player attack method is called, also trigger the animation parameter "Attack"
        stateMachine.OnStateEnter += (state) =>
        {
            if (state.state == ECharacterState.Jumping)
            {
                animator.SetTrigger("Jump");
            }
        };
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnJumpPressed -= HandleJump;
        InputManager.Instance.OnAttackPressed -= HandleAttack;
    }

    private void HandleAttack()
    {
        if (playerStats.damage <= 0 || (stateMachine.rb.constraints == RigidbodyConstraints2D.FreezeAll || (DateTime.Now - lastAttack).TotalMilliseconds <= attackCooldown * 1000))
            return;

        // get enemies in range
        // deal damage to enemies
        // trigger animation
        var hit = Physics2D.OverlapCircleAll(transform.position, attackRange);
        var hostileHits = hit
            .Select(h =>
            {
                return Util.FindParentWithTag(h.transform, "Hostile");
            })
            .Where(parent => parent != null)
            .Distinct();
        foreach (var hostileHit in hostileHits)
        {
            var enemy = hostileHit;
            var losCheck = Physics2D.Linecast(transform.position, enemy.transform.position, LayerMask.GetMask("Ground"));
            if (losCheck.collider != null)
                continue;

            if (enemy.TryGetComponent(out CharacterStats enemyStats) && enemy.TryGetComponent(out NPCStateMachine enemyStateMachine) && enemy.TryGetComponent(out Rigidbody2D enemyRb))
            {
                enemyStats.TakeDamage(playerStats.damage);

                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                knockbackDirection.y = Mathf.Abs(knockbackDirection.y) + 1f; // Slightly adjust the angle upwards
                knockbackDirection.Normalize();
                enemyStateMachine.EnterState(ECharacterState.Decelerating);
                enemyRb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);

            }
        }
        animator.SetTrigger("Attack");
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
}