using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class State<EState> : ScriptableObject where EState : Enum
{
    [HideInInspector]
    public EState state;
    // parameter is the duration from the start of the state to the end of the state
    public UnityEvent<float> OnStateEnd { get; private set; } = new();
    protected StateMachine<EState> stateMachine;

    public State(EState state)
    {
        this.state = state;
    }

    public void SetStateMachine(StateMachine<EState> stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void PhysicsUpdate();
    public abstract void Exit();
}
