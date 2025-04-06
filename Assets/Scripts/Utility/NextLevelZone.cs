public class NextLevelZone : EditorZone<NextLevelZone>
{

    protected override void Start()
    {
        base.Start();
        OnActivate.AddListener((go) =>
        {
            LevelManager.Instance.NextLevel();
        });
    }
}
