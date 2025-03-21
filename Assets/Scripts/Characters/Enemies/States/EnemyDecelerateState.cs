
using UnityEngine;

// TODO: Decelerate, Accelerate and Move share a lot of code, create a base class for them to share functionality
[CreateAssetMenu(fileName = "DecelerateState", menuName = "StateMachines/States/Enemy/DecelerateState")]
public class EnemyDecelerateState : EnemyState
{
    private float startVelocity;
    private float decelerationRate;
    [Header("Debug")]
    public bool debugLedgeDetection = false;

    public EnemyDecelerateState() : base(EEnemyState.Decelerating)
    {
    }

    protected override void OnEnter()
    {
        startVelocity = rb.velocity.x;
    }

    protected override float? OnLoop()
    {
        return null;
    }

    protected override void OnExit()
    {
    }

    protected override float? OnPhysicsUpdate()
    {
        var ground = ((EnemyStateMachine)stateMachine).ground;
        var ledgeDecelerationFactor = CalculateLedgeDecelerationFactor(ground);
        var targetVelocity = GetDesiredHorizontalVelocity(); // Target velocity we want to achieve (0 when not on moving ground)
        decelerationRate = ground.connected ? Config.GroundDecelerationRate : Config.AirDecelerationRate;


        var target = Mathf.Lerp(rb.velocity.x, targetVelocity, ledgeDecelerationFactor * decelerationRate * Time.fixedDeltaTime);
        rb.velocity = new Vector2(
            target,
            rb.velocity.y
        );
        var currentProgress = Mathf.InverseLerp(startVelocity, targetVelocity, ground.relativeVelocity.x);
        var progressDelta = currentProgress - Progress;
        return progressDelta < 0 ? -1 : currentProgress;
    }
    // Handle ledge detection
    private float CalculateLedgeDecelerationFactor(PlayerBoundaryElement ground)
    {
        if (!ground.connected)
            return 1;

        var rayOrigin = new Vector2(rb.position.x + ((EnemyStateMachine)stateMachine).bodyCollider.bounds.size.x * 1.5f * Mathf.Sign(rb.velocity.x), ((EnemyStateMachine)stateMachine).bodyCollider.bounds.min.y);

        RaycastHit2D ledgeHit = Physics2D.Raycast(
            rayOrigin,
            -rb.transform.up,
            Config.LedgeDepthThreshold,
            Config.GroundLayer
        );

        if (debugLedgeDetection) Debug.DrawRay(
            rayOrigin,
            -rb.transform.up * Config.LedgeDepthThreshold,
            ledgeHit.collider ? Color.green : Color.red
        );

        if (ledgeHit.collider == null)
        {
            // There is a ledge and deceleration should be multiplied be the inverse distance
            return Config.LedgeDecelerationFactor;
        }
        return 1;
    }
}