using Manager;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Manager<InputManager>
{
    private PlayerInput PlayerInput;

    public Vector2 Movement { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpCharged { get; private set; } // Maybe Implement a system where you can charge your jump with the character visually charging the jump too, could be a nice mechanic
    public bool JumpReleased { get; private set; }
    public bool CrouchHeld { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool AttackCharged { get; private set; } // Same for attacks, charge attacks are a common mechanic
    public bool AttackReleased { get; private set; }

    // Gets called after Awake in the Manager class
    protected override void Init()
    {
        PlayerInput = GetComponent<PlayerInput>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        Movement = context.action.ReadValue<Vector2>();
    }
    public void Jump(InputAction.CallbackContext context)
    {
        JumpPressed = context.action.WasPressedThisFrame();
        JumpCharged = context.action.ReadValue<float>() > 0;
        JumpReleased = context.action.WasReleasedThisFrame();
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        CrouchHeld = context.action.ReadValue<float>() > 0;
    }
    public void Attack(InputAction.CallbackContext context)
    {
        AttackPressed = context.action.WasPressedThisFrame();
        AttackCharged = context.action.ReadValue<float>() > 0;
        AttackReleased = context.action.WasReleasedThisFrame();
    }

    // Update is called once per frame
    void Update()
    {
        return;
        // Movement = moveAction.ReadValue<Vector2>();
        // JumpPressed = jumpAction.WasPressedThisFrame();
        // JumpCharged = jumpAction.ReadValue<float>() > 0;
        // JumpReleased = jumpAction.WasReleasedThisFrame();
        // CrouchHeld = crouchAction.ReadValue<float>() > 0;
        // AttackPressed = attackAction.WasPressedThisFrame();
        // AttackCharged = attackAction.ReadValue<float>() > 0;
        // AttackReleased = attackAction.WasReleasedThisFrame();
    }
}
