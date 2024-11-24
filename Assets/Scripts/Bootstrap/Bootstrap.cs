using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Bootstrap
{
    private static readonly string baseSceneName = "BaseScene";
    // This method is called whenever a scene is loaded, before any Start or Awake methods are called in that scene
    // It injects the base scene into the scene being loaded unless it is already loaded
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BeforeSceneLoad()
    {
        Scene baseScene = SceneManager.GetSceneByName(baseSceneName);
        if (!baseScene.IsValid())
        {
            SceneManager.LoadScene(baseSceneName, LoadSceneMode.Additive);
        }
    }
}