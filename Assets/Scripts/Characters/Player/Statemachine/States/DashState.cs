using UnityEngine;

[CreateAssetMenu(fileName = "DashState", menuName = "StateMachines/States/Character/DashState")]
public class DashState : CharacterState
{
    private float cachedGravityScale = 1;
    private Vector2 dashDirection;

    public DashState() : base(ECharacterState.Dashing)
    {
    }

    protected override void OnEnter()
    {
        cachedGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        AddDashImpulse();
    }

    protected override float? OnLoop()
    {
        return null;
    }

    protected override void OnExit()
    {
        rb.gravityScale = cachedGravityScale;
    }

    protected override float? OnPhysicsUpdate()
    {
        var currentDirection = rb.velocity.normalized;
        if (dashDirection.normalized != currentDirection.normalized)
        {
            // TODO: exit the state transition to flling (probably progress 0?)
        }
        return null;
    }

    private void AddDashImpulse()
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y); // Unity's gravity is negative
        var impulseMagnitude = Mathf.Sqrt(2 * gravity * Config.MaxJumpHeight) * rb.mass;
        impulseMagnitude = Config.DashDistance * rb.mass;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(-Physics2D.gravity.normalized * impulseMagnitude, ForceMode2D.Impulse);
    }
}