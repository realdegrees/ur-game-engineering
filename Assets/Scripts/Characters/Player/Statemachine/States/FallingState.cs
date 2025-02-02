using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "FallingState", menuName = "StateMachines/States/Character/FallingState")]
public class FallingState : CharacterState
{
    public FallingState() : base(ECharacterState.Falling)
    {
    }

    protected override void OnEnter()
    {
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
        // Check if the player is still ascending e.g.after a cancelled jump
        bool isAscending = Vector2.Dot(rb.velocity.normalized, Physics2D.gravity.normalized) > 0;

        if (!isAscending)
        {
            float verticalVelocityTarget = Mathf.Lerp(rb.velocity.y, Physics2D.gravity.y, Time.fixedDeltaTime);
            rb.velocity = new Vector2(rb.velocity.x, verticalVelocityTarget);
        }

        if (((CharacterStateMachine)stateMachine).ground.connected)
        {
            ((CharacterStateMachine)stateMachine).jumpsSinceGrounded = 0;
            // stateMachine.ExitState(state);
            return 1;
        }
        return null;
    }
}