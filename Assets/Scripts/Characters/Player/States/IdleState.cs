using System;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "StateMachines/States/Character/IdleState")]
public class IdleState : CharacterState
{
    public IdleState() : base(ECharacterState.Idle)
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
        var csm = (CharacterStateMachine)stateMachine;
        if (csm.ground.collider && csm.ground.collider.attachedRigidbody)
        {
            rb.velocity = csm.ground.collider.attachedRigidbody.velocity;
        }

        return null;
    }
}