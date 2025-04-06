using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "MovingState", menuName = "StateMachines/States/NPC/MovingState")]
public class NPCMovingState : NPCState
{
    public NPCMovingState() : base(ECharacterState.Moving)
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
        if (InputManager.Instance.Movement == 0)
        {
            // stateMachine.ExitState(state);
            // stateMachine.EnterState(ECharacterState.Decelerating);
            return 1;
        }
        else
        {
            var targetVelocity = GetDesiredVelocity(); // Target velocity we want to hold
            rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Config.MaxWalkSpeed * Time.fixedDeltaTime);
            return null;
        }
    }
}