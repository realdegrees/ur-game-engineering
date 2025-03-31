using System;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "StateMachines/States/NPC/IdleState")]
public class NPCIdleState : NPCState
{
    public NPCIdleState() : base(ENPCState.Idle)
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
        var csm = (NPCStateMachine)stateMachine;
        if (csm.ground.collider && csm.ground.collider.attachedRigidbody)
        {
            rb.velocity = csm.ground.collider.attachedRigidbody.velocity;
        }

        return null;
    }


}