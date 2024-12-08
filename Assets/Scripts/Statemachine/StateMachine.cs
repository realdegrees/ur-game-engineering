using UnityEngine;
using System.Collections.Generic;
using System;

public class ActiveStates<EState, SConfig> : List<State<EState, SConfig>> where EState : Enum where SConfig : StateMachineConfig<EState, SConfig>
{
    public List<State<EState, SConfig>> Items = new();
}

[Serializable]
public abstract class StateMachine<EState, SConfig> : MonoBehaviour where EState : Enum where SConfig : StateMachineConfig<EState, SConfig>
{
    [SerializeReference]
    public SConfig config;
    public List<State<EState, SConfig>> States => config.states;

    protected ActiveStates<EState, SConfig> activeStates = new();
    private State<EState, SConfig> DefaultState => config.states[0];

    protected virtual void Awake()
    {
        config.states.RemoveAll(state => state == null);

        if (config.states.Count == 0)
        {
            Debug.LogWarning($"No States found in {config.GetType().Name}.");
            return;
        }

        config.states.ForEach(state => state.SetStateMachine(this));

        activeStates.Items.Add(DefaultState);
    }

    protected virtual void Update()
    {
        foreach (var state in activeStates.Items.ToArray())
        {
            state.Loop();
        }
    }
    protected virtual void FixedUpdate()
    {
        foreach (var state in activeStates.Items.ToArray())
        {
            state.PhysicsUpdate();
        }
    }


    public virtual void ExitState(EState state)
    {
        var stateToExit = activeStates.Items.Find(s => s.state.Equals(state));
        if (stateToExit == null)
        {
            if (activeStates.Items.TrueForAll(s => s.runParallel))
            {
                DefaultState.Enter();
                activeStates.Add(DefaultState);
            }
            return;
        }
        stateToExit.Exit();
        activeStates.Items.Remove(stateToExit);

        if (activeStates.Count == 0)
        {
            DefaultState.Enter();
            activeStates.Items.Add(DefaultState);
        }

        Debug.Log($"Exited state {state}");
    }

    /// <summary>
    /// Enters the specified state and adds it to the active states
    /// If the state is not found or already active, an empty array is returned
    /// If the state exists and is not active all non parallel active states are exited and the new state is entered
    /// </summary>
    /// <param name="state">The state to exit.</param>
    /// <param name="alternatives">If the desired state is not setup then try the alternatives</param>
    /// <param name="exclusive">If any of these states are active abort the state activation</param>
    /// <returns>A list of states that were exited</returns>
    public List<EState> EnterState(EState newState, EState[] alternatives = null, EState[] exclusive = null)
    {
        if (exclusive != null && Array.Exists(exclusive, e => e.Equals(newState)))
        {
            return new();
        }

        List<EState> exitedStates = new();

        // Find the matching state in the list of states and return false if it doesn't exist
        var state = States.Find(s => s.state.Equals(newState));
        // If state is not found, try the alternatives
        if (state == null && alternatives != null)
        {
            state = null;
            foreach (var alt in alternatives)
            {
                state = States.Find(s => s.state.Equals(alt));
                if (state != null)
                {
                    break;
                }
            }
        }
        // If state is still not found or active, return
        if (state == null || activeStates.Items.Contains(state))
        {
            return exitedStates;
        }

        // Exit and remove all non-parallel states from activeStates if the state is not parallel
        if (!state.runParallel)
        {
            activeStates.Items.RemoveAll(s =>
            {
                if (!s.runParallel)
                {
                    s.Exit();
                    exitedStates.Add(s.state);
                    return true;
                }
                return false;
            });
        }

        // Enter the new state
        state.SetStartTime(Time.time);
        state.Enter();
        activeStates.Items.Add(state);

        Debug.Log($"Entered state {state.state}");

        return exitedStates;
    }
    protected State<EState, SConfig> GetState(EState state)
    {
        return States.Find(s => s.state.Equals(state));
    }
}