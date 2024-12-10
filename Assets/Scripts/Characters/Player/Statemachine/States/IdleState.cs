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
        // ? Not sure if this should be here or the state machine or a separate component
        // Sticks to moving ground
        var csm = (CharacterStateMachine)stateMachine;
        if (csm.ground.collider)
        {
            if (csm.ground.collider.attachedRigidbody)
                rb.velocity = csm.ground.collider.attachedRigidbody.velocity;
            if (!csm.ground.angleType.Equals(EGroundAngle.Flat))
            {
                rb.velocity = Vector2.zero;
            }


        }

        return null;
        // maybe apply gravity and/or deceleration here?
    }


}