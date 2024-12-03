using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "States/IdleState")]
public class IdleState : CharacterState
{
    public IdleState() : base(ECharacterState.Idle)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    public override void Update()
    {
        Debug.Log("Updating Idle State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }

    public override void PhysicsUpdate()
    {
        throw new NotImplementedException();
    }
}