using System.Collections.Generic;
using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Manager<LevelManager>
{
    public SceneReference baseScene;
    public SceneReference endScene;
    public List<SceneReference> levels_scenario_a = new();
    public List<SceneReference> levels_scenario_b = new();
    private List<SceneReference> levels = new();
    private SceneReference currentLevel;

    private void Start()
    {
        StartCoroutine(SetLevelsFromScenario());
    }

    private IEnumerator SetLevelsFromScenario()
    {
        yield return new WaitUntil(() => GameManager.Instance && GameManager.Instance.Scenario != null);
        levels = GameManager.Instance.Scenario == "A" ? levels_scenario_a : levels_scenario_b;
        currentLevel = levels.Find((level) => level.SceneName == SceneManager.GetActiveScene().name);
    }
    public void ReloadLevel()
    {
        if (currentLevel.SceneName == null) return;
        SceneManager.LoadScene(currentLevel.SceneName);
    }
    public void NextLevel()
    {
        if (levels.Count == 0) return;
        if (currentLevel.SceneName == null)
        {
            currentLevel = levels[0];
            LogManager.Instance.LogLevelStart(currentLevel.SceneName);
        }
        else
        {
            int index = levels.FindIndex(0, levels.Count, (level) => level.SceneName == currentLevel.SceneName);
            if (index == levels.Count - 1)
            {
                LogManager.Instance.LogLevelEnd(currentLevel.SceneName);
                return;
            }

            LogManager.Instance.LogLevelEnd(currentLevel.SceneName);
            currentLevel = levels[index + 1];
            LogManager.Instance.LogLevelStart(currentLevel.SceneName);
        }

        SceneManager.LoadScene(currentLevel.SceneName);
    }

    public SceneReference GetCurrentLevel()
    {
        return currentLevel;
    }
}
