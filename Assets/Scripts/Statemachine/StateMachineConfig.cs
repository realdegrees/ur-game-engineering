using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class StateMachineConfig<EState, T> : ScriptableObject where EState : Enum where T : StateMachineConfig<EState, T>
{
    [Tooltip("The first state in this list is treated as the default state")]
    public List<State<EState, T>> states;
    private List<State<EState, T>> _states = new();
    public List<State<EState, T>> States => _states;

    private void Awake()
    {
        Dictionary<State<EState, T>, State<EState, T>> stateMapping = new();
        foreach (var state in states)
        {
            if (state == null) continue;
            var clone = Instantiate(state);
            stateMapping[state] = clone;
        }

        // for each clone in stateMapping call the UpdateReferences method and pass the dictionairy to it
        foreach (var state in stateMapping.Values)
        {
            state.UpdateReferences(stateMapping);
            _states.Add(state);
        }
    }
}
