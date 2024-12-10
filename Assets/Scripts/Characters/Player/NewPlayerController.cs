using UnityEngine;

[RequireComponent(typeof(CharacterStateMachine))]
public class NewPlayerController : MonoBehaviour
{
    private CharacterStateMachine stateMachine;
    private void Start()
    {
        TryGetComponent(out stateMachine);
        InputManager.Instance.OnJumpPressed += HandleJump;
    }


    private void FixedUpdate()
    {
        HandleMovement();
        HandleFall();
    }

    private void HandleMovement()
    {
        if (InputManager.Instance.Movement == 0)
            return;

        if (stateMachine.IsStateActive(ECharacterState.Moving, ECharacterState.Accelerating, ECharacterState.Decelerating))
            return;


        stateMachine.EnterState(ECharacterState.Accelerating);

        // else
        // {
        //     stateMachine.QueueState(ECharacterState.Accelerating);
        // }


    }
    private void HandleJump()
    {
        // var isGroundedOrFalling = stateMachine.IsStateActive(ECharacterState.Falling, ECharacterState.JumpApex) || stateMachine.ground.connected;
        FallingState fallingState = (FallingState)stateMachine.GetActiveState(ECharacterState.Falling);
        var hasJumpsLeft = stateMachine.jumpsSinceGrounded < stateMachine.config.NumberOfJumpsAllowed;
        var isInCoyoteWindow = fallingState && fallingState.CoyoteTimeActive();
        var canJump = isInCoyoteWindow || (hasJumpsLeft && !isInCoyoteWindow);

        if (canJump)
            stateMachine.EnterState(ECharacterState.Jumping);
    }
    private void HandleFall()
    {
        // TODO: might have to replace with or add a check for the fall state being active
        if (stateMachine.Rb.velocity.y < 0 && !stateMachine.ground.connected && !stateMachine.IsStateActive(ECharacterState.Jumping, ECharacterState.JumpApex))
        {
            stateMachine.jumpsSinceGrounded++;
            stateMachine.EnterState(ECharacterState.Falling);
        }
    }


    private void OnDestroy()
    {
        InputManager.Instance.OnJumpPressed -= HandleJump;
    }
}