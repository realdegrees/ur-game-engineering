using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMovementConfig", menuName = "StateMachines/EnemyMovementConfig")]
public class EnemyMovementConfig : StateMachineConfig<EEnemyState, EnemyMovementConfig>
{
    [Header("Ledge Detection")]
    [Range(.1f, 20)]
    public float LedgeDepthThreshold = 2;
    [Range(1f, 50)]
    public float LedgeDecelerationFactor = 20;
    [Header("Move")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 10f;

    [Range(.1f, 10f)] public float GroundAccelerationRate = .8f;
    [Range(.1f, 10f)] public float GroundDecelerationRate = 2f;
    [Range(.1f, 10f)] public float AirAccelerationRate = .8f;
    [Range(.1f, 10f)] public float AirDecelerationRate = .3f;
    [Range(.5f, 5)] public float GravityMultiplier = 2.5f;
    [Range(1, 10)] public int FollowDistance = 3;
    [Range(1, 10)] public int ResumeDistance = 5;
    [Range(1, 10)] public int WayPointLookAhead = 3;

    [Header("Jump")]
    public float MaxJumpHeight = 6.5f;
    public float MinJumpHeight = .5f;
    [Range(1f, 2f)] public float JumpSpeedMult = 1.2f;
    [Range(0f, 30f)] public float FastFallIntensity = 13f;

    public float jumpForce = 30f;
    public float jumpDetectionDistance = 1f;
    public float pathUpdateDelta = 2f;
    [Tooltip("The horizontal speed at which the character should turn.")]
    [Range(0f, 3f)] public float TurnThreshold = 1f;
    [Header("Ground Detection")]
    public LayerMask GroundLayer;
    public float BottomRange = 0.02f;
    public float TopRange = 0.02f;
    [Header("Ceiling")]
    [Tooltip("The angle threshold at which the player should be pushed down when hitting a ceiling.")]
    [Range(0, 90)]
    public float CeilingAngleThreshold = 25f;
}
