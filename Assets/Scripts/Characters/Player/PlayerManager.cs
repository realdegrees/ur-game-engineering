using UnityEngine;
using System;
using Manager;

public class PlayerManager : Manager<PlayerManager>
{
    [SerializeField]
    private CharacterStateMachine playerStateManager;
}