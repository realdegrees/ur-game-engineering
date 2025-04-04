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

    private GameObject player;
    private GameObject companion;

    private bool companionDead = false;
    private bool playerDead = false;

    private void Update()
    {
        var playerDied = !playerDead && player != null && player.GetComponent<PlayerStats>().GetHealth() <= 0;
        var companionDied = !companionDead && companion != null && companion.GetComponent<NPCStats>().GetHealth() <= 0;
        if (playerDied)
        {
            playerDead = true;
            ReloadLevel();
        }
        else if (companionDied)
        {
            companionDead = true;
            ReloadLevel();
        }
    }
    private void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) => InitializeSceneObjects();
        InitializeSceneObjects();
        StartCoroutine(SetLevelsFromScenario());
    }


    private void InitializeSceneObjects()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        companion = GameObject.FindGameObjectWithTag("Companion");
        playerDead = false;
        companionDead = false;
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
        SceneManager.UnloadSceneAsync(currentLevel.SceneName);
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
