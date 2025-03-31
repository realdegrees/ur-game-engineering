public class CompanionController : NPCController
{
    protected override void Start()
    {
        base.Start();
        stateMachine.OnTargetChanged += (target) =>
        {
            if (target == null)
            {
                stateMachine.SetTarget(PlayerController.Instance.transform);
            }
        };
    }
}