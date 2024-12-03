using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
{
    [SerializeReference]
    protected List<State<EState>> states = new();
    protected List<State<EState>> activeStates = new();

    protected virtual void Awake()
    {
        states.ForEach(state => state.SetStateMachine(this));
        activeStates.Add(states[0]);
    }


    public virtual void EndState(EState state)
    {
        var stateToExit = activeStates.Find(s => s.state.Equals(state));
        if (stateToExit == null) return;
        stateToExit.Exit();
        activeStates.Remove(stateToExit);
    }
    public bool ChangeState(EState newState)
    {
        // Find the matching state in the list of states and return false if it doesn't exist
        var state = states.Find(s => s.state.Equals(newState));
        return true;
    }

    private void Update()
    {
        activeStates.ForEach(state => state.Update());
    }
    private void FixedUpdate()
    {
        activeStates.ForEach(state => state.PhysicsUpdate());
    }
}