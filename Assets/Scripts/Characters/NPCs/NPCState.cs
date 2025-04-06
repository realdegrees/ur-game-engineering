using UnityEngine;

public abstract class NPCState : State<ECharacterState, NPCMovementConfig>
{
    protected Rigidbody2D rb;

    public NPCState(ECharacterState state = ECharacterState.Idle) : base(state) { }
    public void SetRigidbody(Rigidbody2D rb) { this.rb = rb; }

    // TODO calculate desired velocity from path direction
    protected Vector2 GetDesiredVelocity()
    {
        // calculate the direction the player should move in based on the input and the normal of the ground the player is standing on
        var npcStateMachine = (NPCStateMachine)stateMachine;

        var pathDir = Vector2.zero;
        if (npcStateMachine.IsActive)
        {
            pathDir = npcStateMachine.pathDir.normalized;
        }

        var isJumping = stateMachine.IsStateActive(ECharacterState.Jumping);
        var walkSpeed = isJumping ? Config.MaxWalkSpeed * Config.JumpSpeedMult : Config.MaxWalkSpeed;

        if (npcStateMachine.canFly)
        {
            return pathDir.normalized * walkSpeed;
        }

        // Add potentiol moving ground offset
        var ground = npcStateMachine.ground;
        var groundHasRigidbody = ground.collider && ground.collider.attachedRigidbody;
        var offset = groundHasRigidbody ? ground.collider.attachedRigidbody.velocity.x : 0;
        var desiredVelocity = walkSpeed * pathDir.x + offset;
        return new Vector2(desiredVelocity, rb.velocity.y);
    }
}
