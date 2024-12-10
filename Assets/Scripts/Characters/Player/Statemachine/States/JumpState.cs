using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "JumpState", menuName = "StateMachines/States/Character/JumpState")]
public class JumpState : CharacterState
{
    private Vector2 startPosition;
    private bool earlyRelease = false;

    public JumpState() : base(ECharacterState.Jumping)
    {
    }

    protected override void OnEnter()
    {
        ((CharacterStateMachine)stateMachine).jumpsSinceGrounded++;
        earlyRelease = false;
        startPosition = rb.position;
        AddJumpImpulse();

        // Register jump button up listener
        InputManager.Instance.OnJumpReleased += OnJumpReleased;
    }

    protected override float? OnLoop()
    {
        return null;
    }

    protected override void OnExit()
    {
        InputManager.Instance.OnJumpReleased -= OnJumpReleased;
    }

    protected override float? OnPhysicsUpdate()
    {
        bool isPastMinJumpHeight = Vector2.Distance(startPosition, rb.position) > Config.MinJumpHeight;
        bool hitCeiling = ((CharacterStateMachine)stateMachine).ceiling.connected && Config.CeilingAngleThreshold < ((CharacterStateMachine)stateMachine).ceiling.angle;

        // Handle ceiling collisions
        // Handle early jump release (encased in else if to ensure it can't trigger when the player is already at the apex)
        if (hitCeiling || (earlyRelease && isPastMinJumpHeight))
        {
            // stateMachine.EnterState(ECharacterState.Falling);
            // rb.AddForce(Physics2D.gravity.normalized * Config.FastFallIntensity, ForceMode2D.Impulse);
            return -1;
        }

        // Check if vertical velocity (adjusted for gravity direction) is zero and enter falling state
        bool isFalling = Vector2.Angle(rb.velocity, Physics2D.gravity) < 90;
        if (isFalling)
        {
            // stateMachine.EnterState(ECharacterState.JumpApex, new ECharacterState[] { ECharacterState.Falling });
            return 1;
        }

        return Mathf.Abs(rb.position.y - startPosition.y) / Config.MaxJumpHeight;
    }

    private void AddJumpImpulse()
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y); // Unity's gravity is negative
        var impulseMagnitude = Mathf.Sqrt(2 * gravity * Config.MaxJumpHeight) * rb.mass;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(-Physics2D.gravity.normalized * impulseMagnitude, ForceMode2D.Impulse);
    }

    private void OnJumpReleased()
    {
        earlyRelease = true;
    }


}