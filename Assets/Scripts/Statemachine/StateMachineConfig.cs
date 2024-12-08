using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineConfig<EState, T> : ScriptableObject where EState : Enum where T : StateMachineConfig<EState, T>
{
    [Tooltip("The first state in this list is treated as the default state")]
    public List<State<EState, T>> states;
}
