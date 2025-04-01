
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AccelerateState", menuName = "StateMachines/States/NPC/AccelerateState")]
public class NPCAccelerateState : NPCState
{
    private Vector2 startVelocity;
    private float accelerationRate;
    public NPCAccelerateState() : base(ECharacterState.Accelerating)
    {
    }

    protected override void OnEnter()
    {
        // Debug.Log(rb.velocity);
        startVelocity = rb.velocity;
        // Debug.Log(initialSpeed);
    }

    protected override float? OnLoop()
    {
        return null;
    }

    protected override void OnExit()
    {
        if (Progress < 1) return;

        rb.velocity = GetDesiredVelocity();
    }

    protected override float? OnPhysicsUpdate()
    {
        if (!((NPCStateMachine)stateMachine).IsActive)
        {
            // stateMachine.ExitState(state);
            return -1;
        }

        var ground = ((NPCStateMachine)stateMachine).ground;
        var targetVelocity = GetDesiredVelocity(); // Target velocity we want to achieve
        accelerationRate = ground.connected ? Config.GroundAccelerationRate : Config.AirAccelerationRate;

        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, accelerationRate * Time.fixedDeltaTime);
        var currentProgress = Mathf.InverseLerp(startVelocity.magnitude, targetVelocity.magnitude, ground.relativeVelocity.magnitude);
        var progressDelta = currentProgress - Progress;
        return progressDelta < 0 ? -1 : currentProgress;
    }
}