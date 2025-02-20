using UnityEngine;
using System;
using System.Collections;

[Serializable]
public enum ECharacterState
{
    Idle,
    Moving,
    Accelerating,
    Decelerating,
    Jumping,
    JumpApex,
    Attacking,
    Falling,
    Landing,
    Crouching,
    Dashing,
    WallSliding,
}

public abstract class CharacterState : State<ECharacterState, PlayerMovementConfig>
{
    protected Rigidbody2D rb;

    public CharacterState(ECharacterState state = ECharacterState.Idle) : base(state) { }
    public void SetRigidbody(Rigidbody2D rb) { this.rb = rb; }


    protected float GetDesiredHorizontalVelocity()
    {
        // calculate the direction the player should move in based on the input and the normal of the ground the player is standing on
        var cStateMachine = (CharacterStateMachine)stateMachine;

        // Add potentiol moving ground offset
        var ground = cStateMachine.ground;
        var groundHasRigidbody = ground.collider && ground.collider.attachedRigidbody;
        var offset = groundHasRigidbody ? ground.collider.attachedRigidbody.velocity.x : 0;
        var desiredVelocity = Config.MaxWalkSpeed * InputManager.Instance.Movement + offset;
        return desiredVelocity;
    }
}
