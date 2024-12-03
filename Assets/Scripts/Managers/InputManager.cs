using Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : Manager<InputManager>
{
    public UnityEvent onJumpPressed = new();
    public UnityEvent onJumpCharged = new();
    public UnityEvent onJumpReleased = new();
    public UnityEvent onAttackPressed = new();
    public UnityEvent onAttackCharged = new();
    public UnityEvent onAttackReleased = new();
    public UnityEvent onCrouchHeld = new();
    public UnityEvent<float> onMovementStart = new();
    public UnityEvent<float> onMovementEnd = new();

    public bool JumpPressed;
    public bool JumpCharged;
    public bool JumpReleased;
    public bool AttackPressed;
    public bool AttackCharged;
    public bool AttackReleased;
    public bool CrouchHeld;
    public float Movement;

    public void Move(InputAction.CallbackContext context)
    {
        var previousMovement = Movement;
        Movement = context.action.ReadValue<float>();
        if (previousMovement != 0 && Movement == 0)
        {
            onMovementStart?.Invoke(Movement);
        }
        else if (previousMovement == 0 && Movement != 0)
        {
            onMovementEnd?.Invoke(Movement);
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        JumpPressed = context.action.WasPressedThisFrame();
        JumpCharged = context.action.ReadValue<float>() > 0;
        JumpReleased = context.action.WasReleasedThisFrame();

        if (JumpPressed)
            onJumpPressed?.Invoke();
        if (JumpCharged)
            onJumpCharged?.Invoke();
        if (JumpReleased)
            onJumpReleased?.Invoke();

    }

    public void Crouch(InputAction.CallbackContext context)
    {
        CrouchHeld = context.action.ReadValue<float>() > 0;

        if (CrouchHeld)
            onCrouchHeld?.Invoke();
    }
    public void Attack(InputAction.CallbackContext context)
    {
        AttackPressed = context.action.WasPressedThisFrame();
        AttackCharged = context.action.ReadValue<float>() > 0;
        AttackReleased = context.action.WasReleasedThisFrame();

        if (AttackPressed)
            onAttackPressed?.Invoke();
        if (AttackCharged)
            onAttackCharged?.Invoke();
        if (AttackReleased)
            onAttackReleased?.Invoke();
    }
}
