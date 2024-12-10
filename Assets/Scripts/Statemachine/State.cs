using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;

[Serializable]
public abstract class State<EState, SConfig> : BaseState where EState : Enum where SConfig : StateMachineConfig<EState, SConfig>
{
    public EState state { get; private set; }
    protected StateMachine<EState, SConfig> stateMachine;
    protected SConfig Config => stateMachine.config;


    public State(EState state)
    {
        this.state = state;
    }

    public void Init(StateMachine<EState, SConfig> stateMachine)
    {
        this.stateMachine = stateMachine;
        Active = false;
    }

    // ? Events are invoked using reflection to allow for internal logic handling before forwarding the event while neatly hiding the internal methods from subclasses
    #region StateEvents

    public void Loop()
    {
        HandleLoop("OnLoop");
    }
    public void PhysicsUpdate()
    {
        HandleLoop("OnPhysicsUpdate");
    }
    private void HandleLoop(string name)
    {
        Progress = RunSubclassEventImplementation(name) ?? Progress;

        // ! Because the progress value is often lerped it converges on a limit that needs to be offset
        var isCompleted = 1 - Progress < 0.01f;
        if (isCompleted)
        {
            Exit();
            OnStateComplete.Invoke();
        }
        else if (Progress < 0)
        {
            Exit();
            OnStateCancelled.Invoke();
        }
    }
    public void Enter()
    {
        if (enableStateEventLogs) Debug.Log($"Entering {state}");
        stateMachine.OnBeforeStateEnter(state);
        Progress = 0;
        StartTime = Time.time;
        Active = true;
        RunSubclassEventImplementation("OnEnter");
    }
    public void Exit()
    {
        if (enableStateEventLogs) Debug.Log($"Exiting {state}");
        Active = false;
        RunSubclassEventImplementation("OnExit");
        stateMachine.OnAfterStateExit(state);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    private float? RunSubclassEventImplementation(string eventName)
    {
        MethodInfo loopMethod = GetType().GetMethod(eventName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (loopMethod == null)
        {
            Debug.LogError($"Method {eventName} not found in {GetType().Name}");
        }
        var returnValue = loopMethod?.Invoke(this, null);

        return returnValue is float v ? v : null;
    }

    // TODO: Add better description for loop return values
    /// <summary>
    /// Called once when the state is entered
    /// </summary>
    protected abstract void OnEnter();
    /// <summary>
    /// Called in the Update loop
    /// </summary>
    /// <returns>A float that describes the progress of the state between 0 and 1. Return null if you don't want the method to change the progress.</returns>
    protected abstract float? OnLoop();
    /// <summary>
    /// Called in the FixedUpdate loop
    /// </summary>
    /// <returns>A float that describes the progress of the state between 0 and 1. Return null if you don't want the method to change the progress.</returns>
    protected abstract float? OnPhysicsUpdate();
    /// <summary>
    /// Called once when the state is exited
    /// </summary>
    protected abstract void OnExit();

    #endregion
}

