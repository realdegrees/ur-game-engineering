using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public abstract class StateMachine<EState, SConfig> : MonoBehaviour where EState : Enum where SConfig : StateMachineConfig<EState, SConfig>
{
    [SerializeReference]
    private SConfig config;
    private SConfig _config;
    public SConfig Config => _config;
    public List<State<EState, SConfig>> States => _states;
    private List<State<EState, SConfig>> _states = new();

    protected List<State<EState, SConfig>> ActiveStates => States.Where(s => s.Active).ToList();

    private State<EState, SConfig> DefaultState => Config.States[0];
    // private List<EState> queuedStates = new();

    public event Action<State<EState, SConfig>> OnStateExit = delegate { };
    public event Action<State<EState, SConfig>> OnStateEnter = delegate { };

    protected virtual void Awake()
    {
        _config = Instantiate(config);
        _states = Config.States;
        Config.States.RemoveAll(state => state == null);
        if (Config.States.Count == 0)
        {
            Debug.LogWarning($"No States found in {Config.GetType().Name}.");
            return;
        }

        Config.States.ForEach(state =>
        {
            state.Init(this);
        });
        DefaultState.Enter();
    }


    internal void StateExited(State<EState, SConfig> state)
    {
        OnStateExit?.Invoke(state);
        if (ActiveStates.Count == 0 && !state.Equals(DefaultState.state))
        {
            DefaultState.Enter();
        }
    }
    internal void StateEntered(State<EState, SConfig> state)
    {
        OnStateEnter?.Invoke(state);
        if (state != DefaultState && IsStateActive(DefaultState.state))
        {
            DefaultState.Exit();
        }

        // Exit and remove all non-parallel states from activeStates if the state is not parallel
        if (!state.runParallel)
        {
            foreach (var s in ActiveStates)
            {
                if (s != state && !s.runParallel)
                    s.Exit();
            }
        }
    }


    protected virtual void Update()
    {
        // Debug.Log(ActiveStates.Select(s => s.name).ToArray());
        foreach (var state in ActiveStates)
        {
            state.Loop();
        }
    }
    protected virtual void FixedUpdate()
    {
        foreach (var state in ActiveStates)
        {
            state.PhysicsUpdate();
        }
    }

    public State<EState, SConfig> GetActiveState(EState state)
    {
        return ActiveStates.Find(s => s.state.Equals(state));
    }


    public virtual void ExitState(EState state)
    {
        var stateToExit = ActiveStates.Find(s => s.state.Equals(state));
        if (stateToExit == null)
        {
            return;
        }
        stateToExit.Exit();

    }




    /// <param name="state">The state to exit.</param>
    /// <param name="exclusive">If any of these states are active abort the state activation</param>
    /// <returns>A list of states that were exited</returns>
    public void EnterState(EState newState, EState[] exclusive = null)
    {
        if (exclusive != null && Array.Exists(exclusive, e => e.Equals(newState)))
        {
            return;
        }

        List<EState> exitedStates = new();

        // Find the matching state in the list of states and return false if it doesn't exist
        var state = States.Find(s => s.state.Equals(newState));

        // If state is not found or active, return
        if (state == null || ActiveStates.Contains(state))
        {
            return;
        }

        // // Check if the default state is currently running and exit it
        // var defaultState = DefaultState;
        // if (ActiveStates.Contains(defaultState))
        // {
        //     defaultState.Exit();
        //     ActiveStates.Remove(defaultState);
        //     exitedStates.Add(defaultState.state);
        // }

        // // Exit and remove all non-parallel states from activeStates if the state is not parallel
        // if (!state.runParallel)
        // {
        //     var statesToRemove = ActiveStates.Where(s => !s.runParallel).ToList();
        //     foreach (var s in statesToRemove)
        //     {
        //         s.Exit();
        //         exitedStates.Add(s.state);
        //         ActiveStates.Remove(s);
        //     }
        // }

        // Enter the new state
        state.Enter();
    }
    protected State<EState, SConfig> GetState(EState state)
    {
        return States.Find(s => s.state.Equals(state));
    }
    public bool IsStateActive(params EState[] states)
    {
        return states.Any(state => ActiveStates.Any(s => s.state.Equals(state)));
    }
    // /// <summary>
    // /// Queues the state and enters it the next time any state is exited
    // /// </summary>
    // public void QueueState(EState state)
    // {
    //     queuedStates.Add(state);
    // }
}