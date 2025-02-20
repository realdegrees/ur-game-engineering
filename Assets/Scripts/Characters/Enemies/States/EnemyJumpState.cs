using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "JumpState", menuName = "StateMachines/States/Enemy/JumpState")]
public class EnemyJumpState : EnemyState
{
    private Vector2 startPosition;

    public EnemyJumpState() : base(EEnemyState.Jumping)
    {
    }

    protected override void OnEnter()
    {
        ((EnemyStateMachine)stateMachine).jumpsSinceGrounded++;
        startPosition = rb.position;
        AddJumpImpulse();

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
        bool isPastMinJumpHeight = Vector2.Distance(startPosition, rb.position) > Config.MinJumpHeight;
        bool hitCeiling = ((EnemyStateMachine)stateMachine).ceiling.connected && Config.CeilingAngleThreshold < ((EnemyStateMachine)stateMachine).ceiling.angle;

        // Handle ceiling collisions
        // Handle early jump release (encased in else if to ensure it can't trigger when the player is already at the apex)
        if (hitCeiling || isPastMinJumpHeight)
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

}