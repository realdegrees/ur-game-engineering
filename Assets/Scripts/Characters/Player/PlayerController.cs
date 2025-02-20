using System;
using UnityEngine;

[RequireComponent(typeof(CharacterStateMachine))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public CharacterStateMachine stateMachine;

    public event Action OnFlip = delegate { };

    public bool IsFacingRight { get; private set; } = true;

    private void Start()
    {
        TryGetComponent(out stateMachine);
        InputManager.Instance.OnJumpPressed += HandleJump;
    }
    private void Update()
    {
        FlipCheck();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleFall();
    }

    private void FlipCheck()
    {
        if (InputManager.Instance.Movement < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            transform.rotation = Quaternion.Euler(0f, 0, 0f);
            OnFlip.Invoke();
        }
        else if (InputManager.Instance.Movement > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
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


    private void OnDestroy()
    {
        InputManager.Instance.OnJumpPressed -= HandleJump;
    }
}