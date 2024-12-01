using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Movement Config")]
public class PlayerMovementConfig : ScriptableObject
{
    [Header("Move")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 10f;
    [Range(0.25f, 40f)] public float GroundAcceleration = 4f;
    [Range(0.25f, 40f)] public float GroundDeceleration = 18f;
    [Range(0.25f, 40f)] public float AirAcceleration = 4f;
    [Range(0.25f, 40f)] public float AirDeceleration = 18f;
    [Tooltip("The horizontal speed at which the character should turn.")]
    [Range(0f, 3f)] public float TurnThreshold = 1f;
    [Header("Slopes")]
    [Range(1, 80)] public int MaxSlopeAngle = 50;
    [Tooltip("Speed multiplier when walking up a slope.")]
    [Range(0f, 2f)] public float DownSlopeSpeedMultiplier = 0.7f;
    [Tooltip("Speed multiplier when walking down a slope.")]
    [Range(0f, 2f)] public float UpSlopeSpeedMultiplier = 1.3f;
    [Tooltip("How much the angle of the slope should influence the speed.")]
    [Range(0f, 1f)] public float SlopeSteepnessFactor = .2f;
    [Range(0f, 1f)] public float SlopeRejectionForce = .5f;
    [Header("Ground Detection")]
    public LayerMask GroundLayer;
    public float BottomRange = 0.02f;
    public float TopRange = 0.02f;


    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(0f, 1f)] public float TimeTillJumpApex = 0.35f; // TODO: calculate this from the project gravity instead
    public float MaxFallSpeed = 26f;
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    [Range(1, 5)] public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.8f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

    [Header("Jump Visualization")]
    public bool Show = false;
    [Range(5, 100)] public int ArcResolution = 100;
    [Range(0, 500)] public int VisualizationSteps = 150;

    [Header("Debug")]
    public bool CollisionGizmos = true;
    public bool VelocityGizmos = true;

    public float Gravity { get; private set; } // TODO: Use Project Gravity instead
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
