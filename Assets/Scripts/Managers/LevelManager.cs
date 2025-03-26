using System.Collections.Generic;
using Manager;
using UnityEngine.SceneManagement;

public class LevelManager : Manager<LevelManager>
{
    public SceneReference baseScene;
    public SceneReference endScene;
    public List<SceneReference> levels = new();
    private SceneReference currentLevel;

    protected override void Awake()
    {
        base.Awake();
        currentLevel = levels.Find((level) => level.SceneName == SceneManager.GetActiveScene().name);
    }
    public void NextLevel()
    {
        if (levels.Count == 0) return;
        if (currentLevel.SceneName == null)
        {
            currentLevel = levels[0];
        }
        else
        {
            int index = levels.FindIndex(0, levels.Count, (level) => level.SceneName == currentLevel.SceneName);
            if (index == levels.Count - 1) return;
            currentLevel = levels[index + 1];
        }

        SceneManager.LoadScene(currentLevel.SceneName);
    }

    public SceneReference GetCurrentLevel()
    {
        return currentLevel;
    }
}
