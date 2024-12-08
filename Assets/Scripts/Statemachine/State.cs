using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public abstract class State<EState, SConfig> : ScriptableObject where EState : Enum where SConfig : StateMachineConfig<EState, SConfig>
{
    public EState state { get; private set; }
    public bool runParallel = false;
    // parameter is the duration from the start of the state to the end of the state
    public UnityEvent<float> OnStateEnd { get; private set; } = new();
    protected StateMachine<EState, SConfig> stateMachine;
    protected SConfig Config => stateMachine.config;
    protected float startTime;

    public State(EState state)
    {
        this.state = state;
    }

    public void SetStateMachine(StateMachine<EState, SConfig> stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void SetStartTime(float startTime)
    {
        this.startTime = startTime;
    }

    public abstract void Enter();
    public abstract void Loop();
    public abstract void PhysicsUpdate();
    public abstract void Exit();
}
