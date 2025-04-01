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
            var targetVelocity = GetDesiredHorizontalVelocity(); // Target velocity we want to hold
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(targetVelocity, rb.velocity.y), Config.MaxWalkSpeed * Time.fixedDeltaTime);
            return null;
        }
    }
}