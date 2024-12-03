using UnityEngine;
using System;

[Serializable]
public enum ECharacterState
{
    Idle,
    Moving,
    Jumping,
    Attacking,
    Falling,
    Landing,
    Crouching,
    Dashing,
    WallSliding,
}

public abstract class CharacterState : State<ECharacterState>
{
    [SerializeField]
    protected Rigidbody2D rb;

    public CharacterState(ECharacterState state = ECharacterState.Idle) : base(state) { }
    public void SetRigidbody(Rigidbody2D rb) { this.rb = rb; }
}
