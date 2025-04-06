using System;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "StateMachines/States/NPC/IdleState")]
public class NPCIdleState : NPCState
{
    public NPCIdleState() : base(ECharacterState.Idle)
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
        if (csm.rb.velocity.magnitude > 0.1f)
        {
            Exit();
            csm.EnterState(ECharacterState.Decelerating);
        }
        return null;
    }


}