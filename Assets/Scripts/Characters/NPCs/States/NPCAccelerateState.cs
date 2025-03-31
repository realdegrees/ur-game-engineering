
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AccelerateState", menuName = "StateMachines/States/NPC/AccelerateState")]
public class NPCAccelerateState : NPCState
{
    private float startVelocity;
    private float accelerationRate;
    public NPCAccelerateState() : base(ENPCState.Accelerating)
    {
    }

    protected override void OnEnter()
    {
        // Debug.Log(rb.velocity);
        startVelocity = rb.velocity.x;
        // Debug.Log(initialSpeed);
    }

    protected override float? OnLoop()
    {
        return null;
    }

    protected override void OnExit()
    {
        if (Progress < 1) return;

        var targetVelocity = GetDesiredHorizontalVelocity(); // Target velocity we want to hold
        rb.velocity = new Vector2(targetVelocity, rb.velocity.y);
    }

    protected override float? OnPhysicsUpdate()
    {
        if (!((NPCStateMachine)stateMachine).IsActive)
        {
            // stateMachine.ExitState(state);
            return -1;
        }

        var ground = ((NPCStateMachine)stateMachine).ground;
        var targetVelocity = GetDesiredHorizontalVelocity(); // Target velocity we want to achieve
        accelerationRate = ground.connected ? Config.GroundAccelerationRate : Config.AirAccelerationRate;

        var target = Mathf.Lerp(rb.velocity.x, targetVelocity, accelerationRate * Time.fixedDeltaTime);
        rb.velocity = new Vector2(
            target,
            rb.velocity.y
        );
        var currentProgress = Mathf.InverseLerp(startVelocity, targetVelocity, ground.relativeVelocity.x);
        var progressDelta = currentProgress - Progress;
        return progressDelta < 0 ? -1 : currentProgress;
    }
}