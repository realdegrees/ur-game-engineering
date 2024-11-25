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

    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction jumpAction;
    private InputAction attackAction;

    protected override void Awake()
    {
        base.Awake();

        PlayerInput = GetComponent<PlayerInput>();

        moveAction = PlayerInput.actions["Move"];
        crouchAction = PlayerInput.actions["Crouch"];
        jumpAction = PlayerInput.actions["Jump"];
        attackAction = PlayerInput.actions["Attack"];
    }

    // Update is called once per frame
    void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
        JumpPressed = jumpAction.triggered;
        JumpCharged = jumpAction.phase == InputActionPhase.Performed;
        JumpReleased = jumpAction.phase == InputActionPhase.Canceled;
        CrouchHeld = crouchAction.ReadValue<float>() > 0;
        AttackPressed = attackAction.triggered;
        AttackCharged = attackAction.phase == InputActionPhase.Performed;
        AttackReleased = attackAction.phase == InputActionPhase.Canceled;
    }
}
