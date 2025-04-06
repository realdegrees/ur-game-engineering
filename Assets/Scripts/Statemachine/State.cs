using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;
using System.Collections.Generic;

[Serializable]
public abstract class State<EState, SConfig> : BaseState where EState : Enum where SConfig : StateMachineConfig<EState, SConfig>
{
    public EState state { get; private set; }
    protected StateMachine<EState, SConfig> stateMachine;
    protected SConfig Config => stateMachine.Config;
    public State<EState, SConfig> enterOnComplete = null;
    public State<EState, SConfig> enterOnCancel = null;

    public State(EState state)
    {
        this.state = state;
    }

    public void UpdateReferences(Dictionary<State<EState, SConfig>, State<EState, SConfig>> stateMapping)
    {
        if (enterOnComplete != null)
            enterOnComplete = stateMapping.ContainsKey(enterOnComplete) ? stateMapping[enterOnComplete] : enterOnComplete;
        if (enterOnCancel != null)
            enterOnCancel = stateMapping.ContainsKey(enterOnCancel) ? stateMapping[enterOnCancel] : enterOnCancel;
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
        HandleLoop(nameof(OnLoop));
    }
    public void PhysicsUpdate()
    {
        HandleLoop(nameof(OnPhysicsUpdate));
    }
    private void HandleLoop(string name)
    {
        Progress = RunSubclassEventImplementation(name) ?? Progress;

        // ! Because the progress value is often lerped it converges on a limit that needs to be offset
        var isCompleted = 1 - Progress < 0.01f;
        if (isCompleted)
        {
            Exit();
            if (enterOnComplete != null)
                enterOnComplete.Enter();
        }
        else if (Progress < 0)
        {
            Exit();
            if (enterOnCancel != null)
                enterOnCancel.Enter();
        }
    }
    public void Enter()
    {
        if (enableStateEventLogs) Debug.Log($"Entering {state}");
        Progress = 0;
        StartTime = Time.time;
        Active = true;
        stateMachine.StateEntered(this);
        RunSubclassEventImplementation("OnEnter");
    }
    public void Exit()
    {
        if (enableStateEventLogs) Debug.Log($"Exiting {state}");
        Active = false;
        RunSubclassEventImplementation("OnExit");
        stateMachine.StateExited(this);
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

