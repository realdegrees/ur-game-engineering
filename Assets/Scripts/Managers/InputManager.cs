using System;
using Manager;
using UnityEngine.InputSystem;

public class InputManager : Manager<InputManager>
{
    public event Action OnJumpPressed = delegate { };
    public event Action OnJumpReleased = delegate { };
    public event Action OnAttackPressed = delegate { };
    public event Action OnAttackReleased = delegate { };
    public event Action OnCrouchPressed = delegate { };
    public event Action OnCrouchReleased = delegate { };
    public event Action<float> OnMovementStart = delegate { };
    public event Action<float> OnMovementEnd = delegate { };
    public event Action<float> OnMovementFlip = delegate { };

    public bool JumpPressed;
    public bool JumpHeld;
    public bool JumpReleased;
    public bool AttackPressed;
    public bool AttackHeld;
    public bool AttackReleased;
    public bool CrouchHeld;
    public bool CrouchPressed;
    public bool CrouchReleased;
    public float Movement;

    public void Move(InputAction.CallbackContext context)
    {
        var previousMovement = Movement;
        Movement = context.action.ReadValue<float>();

        InvokeIfTrue(previousMovement != 0 && Movement == 0, () => OnMovementEnd?.Invoke(Movement));
        InvokeIfTrue(previousMovement == 0 && Movement != 0, () => OnMovementStart?.Invoke(Movement));
        InvokeIfTrue(previousMovement == -Movement, () => OnMovementFlip?.Invoke(Movement));
    }
    public void Jump(InputAction.CallbackContext context)
    {
        JumpPressed = context.action.WasPressedThisFrame();
        JumpReleased = context.action.WasReleasedThisFrame();
        JumpHeld = !(JumpHeld && JumpReleased) || JumpPressed;

        InvokeIfTrue(JumpPressed, OnJumpPressed);
        InvokeIfTrue(JumpReleased, OnJumpReleased);
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        CrouchPressed = context.action.WasPressedThisFrame();
        CrouchReleased = context.action.WasReleasedThisFrame();
        CrouchHeld = !(CrouchHeld && CrouchReleased) || CrouchPressed;

        InvokeIfTrue(CrouchPressed, OnCrouchPressed);
        InvokeIfTrue(CrouchReleased, OnCrouchReleased);
    }
    public void Attack(InputAction.CallbackContext context)
    {
        AttackPressed = context.action.WasPressedThisFrame();
        AttackReleased = context.action.WasReleasedThisFrame();
        AttackHeld = !(AttackHeld && AttackReleased) || AttackPressed;

        InvokeIfTrue(AttackPressed, OnAttackPressed);
        InvokeIfTrue(AttackReleased, OnAttackReleased);
    }

    private void InvokeIfTrue(bool condition, Action inputEvent)
    {
        if (condition)
            inputEvent.Invoke();
    }
}
