using System;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "StateMachines/States/Enemy/IdleState")]
public class EnemyIdleState : EnemyState
{
    public EnemyIdleState() : base(EEnemyState.Idle)
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
        var csm = (EnemyStateMachine)stateMachine;
        if (csm.ground.collider && csm.ground.collider.attachedRigidbody)
        {
            rb.velocity = csm.ground.collider.attachedRigidbody.velocity;
        }

        return null;
    }


}