using UnityEngine;
using System;
using System.Collections;

[Serializable]
public enum EEnemyState
{
    Idle,
    Moving,
    Accelerating,
    Decelerating,
    Jumping,
    Attacking,
    Falling,
}

public abstract class EnemyState : State<EEnemyState, EnemyMovementConfig>
{
    protected Rigidbody2D rb;

    public EnemyState(EEnemyState state = EEnemyState.Idle) : base(state) { }
    public void SetRigidbody(Rigidbody2D rb) { this.rb = rb; }

    // TODO calculate desired velocity from path direction
    protected float GetDesiredHorizontalVelocity()
    {
        // calculate the direction the player should move in based on the input and the normal of the ground the player is standing on
        var eStateMachine = (EnemyStateMachine)stateMachine;

        var pathDir = Vector2.zero;
        if (eStateMachine.IsActive)
        {
            pathDir = eStateMachine.pathDir.normalized;
        }

        // Add potentiol moving ground offset
        var ground = eStateMachine.ground;
        var groundHasRigidbody = ground.collider && ground.collider.attachedRigidbody;
        var offset = groundHasRigidbody ? ground.collider.attachedRigidbody.velocity.x : 0;
        var isJumping = stateMachine.IsStateActive(EEnemyState.Jumping);
        var walkSpeed = isJumping ? Config.MaxWalkSpeed * Config.JumpSpeedMult : Config.MaxWalkSpeed;
        var desiredVelocity = walkSpeed * pathDir.x + offset;
        return desiredVelocity;
    }
}
