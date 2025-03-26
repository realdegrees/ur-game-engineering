public class NextLevelZone : EditorZone<NextLevelZone>
{

    protected override void Start()
    {
        base.Start();
        OnActivate.AddListener(() =>
        {
            LevelManager.Instance.NextLevel();
        });
    }
}
