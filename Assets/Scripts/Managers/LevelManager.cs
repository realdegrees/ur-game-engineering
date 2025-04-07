using System.Collections.Generic;
using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.XR;

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
            companionDead = true;
            StartCoroutine(TransitionDeathUI(UIManager.Instance.deathBackground, UIManager.Instance.deathText, "You died"));
        }
        else if (companionDied)
        {
            companionDead = true;
            playerDead = true;
            StartCoroutine(TransitionDeathUI(UIManager.Instance.deathBackground, UIManager.Instance.deathText, "Your companion died"));
        }
    }
    private IEnumerator TransitionDeathUI(Image deathBackground, TextMeshProUGUI deathText, string text)
    {
        float duration = 3f;
        float elapsedTime = 0f;

        Color backgroundColor = deathBackground.color;
        Color textColor = deathText.color;
        deathText.text = text;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);

            backgroundColor.a = alpha;
            textColor.a = alpha;

            deathBackground.color = backgroundColor;
            deathText.color = textColor;

            yield return null;
        }
        ReloadLevel();

    }
    private void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) => InitLevel(scene);
        StartCoroutine(SetLevelsFromScenario());
    }
    private void InitLevel(Scene scene)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        companion = GameObject.FindGameObjectWithTag("Companion");
        playerDead = false;
        companionDead = false;

        bool isLevelScene = levels.Any(level => level.SceneName == scene.name);
        bool isTransitionScene = scene.name.ToLower().Contains("transition");
        if (isLevelScene && !isTransitionScene)
        {
            LogManager.Instance.LogLevelStart(currentLevel.SceneName);
        }
    }

    private IEnumerator SetLevelsFromScenario()
    {
        yield return new WaitUntil(() => GameManager.Instance && GameManager.Instance.Scenario != null);
        levels = GameManager.Instance.Scenario == "A" ? levels_scenario_a : levels_scenario_b;
        currentLevel = levels.Find((level) => level.SceneName == SceneManager.GetActiveScene().name);

        HandleLogOnSceneLoad();
    }

    private void HandleLogOnSceneLoad()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            bool isLevelScene = levels.Any(level => level.SceneName == scene.name);
            bool isTransitionScene = scene.name.ToLower().Contains("transition");
            if (isLevelScene && !isTransitionScene)
            {
                LogManager.Instance.LogLevelStart(currentLevel.SceneName);
                break;
            }
        }
    }
    public void ReloadLevel()
    {
        if (currentLevel.SceneName == null) return;
        SceneManager.UnloadSceneAsync(currentLevel.SceneName);
        SceneManager.LoadScene(currentLevel.SceneName);
    }
    public void NextLevel()
    {
        StartCoroutine(NextLevelCoroutine());
    }

    IEnumerator NextLevelCoroutine()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (currentLevel.SceneName == null)
        {
            currentLevel = levels[0];
            SceneManager.LoadScene(currentLevel.SceneName);
            yield break;
        }

        int index = levels.FindIndex(0, levels.Count, (level) => level.SceneName == currentLevel.SceneName);
        if (index == levels.Count - 1)
        {
            if (!currentLevel.SceneName.ToLower().Contains("transition")) LogManager.Instance.LogLevelEnd(currentLevel.SceneName);
            SceneManager.LoadScene(endScene.SceneName);
        }
        else
        {
            if (!currentLevel.SceneName.ToLower().Contains("transition")) LogManager.Instance.LogLevelEnd(currentLevel.SceneName);
            currentLevel = levels[index + 1];

            SceneManager.LoadScene(currentLevel.SceneName);
        }
    }

    public SceneReference GetCurrentLevel()
    {
        return currentLevel;
    }
}
