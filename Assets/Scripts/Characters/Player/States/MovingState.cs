using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MovingState", menuName = "States/MovingState")]
public class MovingState : CharacterState
{
    public MovingState() : base(ECharacterState.Moving)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Moving State");
    }

    public override void Update()
    {
        Debug.Log("Updating Moving State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Moving State");
    }

    public override void PhysicsUpdate()
    {
        var movement = InputManager.Instance.Movement;
        if (movement == 0)
        {
            return;
        }
        Debug.Log("Moving");
    }
}