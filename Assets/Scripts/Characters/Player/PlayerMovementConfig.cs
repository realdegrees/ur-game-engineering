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

    [Header("Run")]
    [Range(1f, 100f)] public float MaxRunSpeed = 20f;

    [Header("Ground Detection")]
    public LayerMask GroundLayer;
    public float BottomRange = 0.02f;
    public float TopRange = 0.02f;
    [Range(0.1f, 1f)] public float HeadWidth = 0.5f;

    [Header("Debug")]
    public bool DrawGizmos = true;
}
