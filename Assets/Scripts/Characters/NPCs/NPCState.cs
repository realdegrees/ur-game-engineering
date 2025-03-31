using UnityEngine;
using System;
using System.Collections;

[Serializable]
public enum ENPCState
{
    Idle,
    Moving,
    Accelerating,
    Decelerating,
    Jumping,
    Attacking,
    Falling,
}

public abstract class NPCState : State<ENPCState, NPCMovementConfig>
{
    protected Rigidbody2D rb;

    public NPCState(ENPCState state = ENPCState.Idle) : base(state) { }
    public void SetRigidbody(Rigidbody2D rb) { this.rb = rb; }

    // TODO calculate desired velocity from path direction
    protected float GetDesiredHorizontalVelocity()
    {
        // calculate the direction the player should move in based on the input and the normal of the ground the player is standing on
        var npcStateMachine = (NPCStateMachine)stateMachine;

        var pathDir = Vector2.zero;
        if (npcStateMachine.IsActive)
        {
            pathDir = npcStateMachine.pathDir.normalized;
        }

        // Add potentiol moving ground offset
        var ground = npcStateMachine.ground;
        var groundHasRigidbody = ground.collider && ground.collider.attachedRigidbody;
        var offset = groundHasRigidbody ? ground.collider.attachedRigidbody.velocity.x : 0;
        var isJumping = stateMachine.IsStateActive(ENPCState.Jumping);
        var walkSpeed = isJumping ? Config.MaxWalkSpeed * Config.JumpSpeedMult : Config.MaxWalkSpeed;
        var desiredVelocity = walkSpeed * pathDir.x + offset;
        return desiredVelocity;
    }
}
