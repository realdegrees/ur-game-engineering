using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Movement Config")]
public class PlayerMovementConfig : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 10f;
    [Range(0.25f, 40f)] public float GroundAcceleration = 4f;
    [Range(0.25f, 40f)] public float GroundDeceleration = 18f;
    [Range(0.25f, 40f)] public float AirAcceleration = 4f;
    [Range(0.25f, 40f)] public float AirDeceleration = 18f;
    [Tooltip("The horizontal speed at which the character should turn.")]
    [Range(0f, 1f)] public float TurnThreshold = .2f;

    [Header("Ground Detection")]
    public LayerMask GroundLayer;
    public float BottomRange = 0.02f;
    public float TopRange = 0.02f;


    [Header("Jump")]
    public float JumpHeight = 6.5f;
    public float TimeTillJumpApex = 0.35f;
    public float MaxFallSpeed = 26f;
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    [Range(1, 5)] public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

    [Header("Jump Visualization")]
    public bool ShowJumpArc = false;
    public bool StopOnCollision = true;
    public bool DrawRight = true;
    [Range(5, 100)] public int ArcResolution = 20;
    [Range(0, 500)] public int VisualizationSteps = 90;

    [Header("Debug")]
    public bool DrawGizmos = true;

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    public float AdjustedJumpHeight { get; private set; }

    private void OnValidate()
    {
        CalculateValues();
    }
    private void OnEnable()
    {
        CalculateValues();
    }
    private void CalculateValues()
    {
        AdjustedJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }
}
