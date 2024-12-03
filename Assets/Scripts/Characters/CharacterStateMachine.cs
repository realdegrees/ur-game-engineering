using UnityEngine;

public class CharacterStateMachine : StateMachine<ECharacterState>
{
    public Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        states.ForEach(state => ((CharacterState)state).SetRigidbody(rb));
    }

    // TODO: set listeners here for inputmanager and change states according to the properties of the current states (like coyoyte time etc)
    // TODO: so that special stuff is handled here while the pure movement and physics calcualtions are done in the states themselves

}