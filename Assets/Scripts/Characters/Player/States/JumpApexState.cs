using UnityEngine;

// ! TODO: fix  apex hang time, it's not working probably due to to falling cutting in early because it doesn't exit itself

[CreateAssetMenu(fileName = "JumpApexState", menuName = "StateMachines/States/Character/JumpApexState")]
public class JumpApexState : CharacterState
{
    private float apexHangDuration = 0;
    private float cachedGravityScale = 0;
    public JumpApexState() : base(ECharacterState.JumpApex)
    {
    }

    protected override void OnEnter()
    {
        cachedGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        apexHangDuration = 0;
        InputManager.Instance.OnJumpReleased += OnJumpEnd;
    }

    private void OnJumpEnd()
    {
        InputManager.Instance.OnJumpReleased -= OnJumpEnd;
        //stateMachine.EnterState(ECharacterState.Falling);
        Progress = 1;
    }

    protected override float? OnLoop()
    {
        var currentProgress = apexHangDuration / Config.ApexHangTime;
        apexHangDuration += Time.deltaTime;
        return currentProgress;
    }

    protected override void OnExit()
    {
        rb.gravityScale = cachedGravityScale;
        // rb.AddForce(Physics2D.gravity.normalized * Config.FastFallIntensityApex, ForceMode2D.Impulse);
    }

    protected override float? OnPhysicsUpdate()
    {
        return null;
    }
}