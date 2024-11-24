#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Manager;
using System.Linq;

public class SceneLoader : Manager<SceneLoader>
{
    [Serializable]
    public struct SceneLoadInfo
    {
#if UNITY_EDITOR
        [Tooltip("The scene to be bootstrapped")]
        public SceneAsset scene;
#endif
        public string name;
        public bool loadAsync;
#if UNITY_EDITOR
        [Tooltip("Do not bootstrap it for these scenes")]
        public List<SceneAsset> exclude;
#endif
        public List<string> _exclude;
    }

#if UNITY_EDITOR
    [SerializeField]
    private SceneAsset menuScene, loadingScene, levels;    
#endif
    private string _menuScene, _loadingScene, _levels;
    [Tooltip("These scenes are additively added every time a new scene loads")]
    [InspectorName("Bootstrap Scenes")]
    public List<SceneLoadInfo> scenes = new();
    // This loaads all scenes defined in bootstrapScenes and loads them sync or async
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        if (scenes.Any(scene => scene.name == loadedScene.name)) return;
        // Sort by async scenes to load them first
        scenes.Sort((x, y) => y.loadAsync.CompareTo(x.loadAsync));

        // Get all active scenes in order to skip a bootstrap scene if an excluded scene is active
        List<string> activeSceneNames = new List<string>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            activeSceneNames.Add(scene.name);
        }
        // Load scenes and save the async operation to loadState if loading async
        foreach (var sceneInfo in scenes)
        {
            // Skip loading if an excluded scene is active
            if (sceneInfo._exclude.Any(excludedScene => activeSceneNames.Contains(excludedScene)))
            {
                continue;
            }

            // Load the bootstrap scene additively if it is not already loaded
            Scene scene = SceneManager.GetSceneByName(sceneInfo.name);
            if (scene.IsValid()) continue;

            if (sceneInfo.loadAsync)
            {
                Debug.Log("Bootstrapping \"" + sceneInfo.name + "\" before \"" + loadedScene.name + "\"");
                var loadState = SceneManager.LoadSceneAsync(sceneInfo.name, LoadSceneMode.Additive);
                loadState.completed += (op) => Debug.Log(sceneInfo.name + " loaded!");
            }
            else
            {
                SceneManager.LoadScene(sceneInfo.name, LoadSceneMode.Additive);
                Debug.Log(sceneInfo.name + " loaded!");
            }
        }
    }

    // This custom inspector allows for the scenes to be dragged in the editor instead of hardcoding the name
    // source: https://stackoverflow.com/questions/74333318/the-type-or-namespace-name-sceneasset-could-not-be-found-error-when-trying-t
#if UNITY_EDITOR
    public void OnAfterDeserialize() => FillScenes();
    public void OnBeforeSerialize() => FillScenes();
    public void OnValidate() => FillScenes();

    private void FillScenes()
    {
        scenes.ForEach(sceneInfo =>
        {
            if (sceneInfo.scene != null)
            {
                sceneInfo.name ??= sceneInfo.scene.name;
            }
            if (sceneInfo._exclude != null)
            {
                sceneInfo._exclude = sceneInfo.exclude?.Where(scene => scene != null).Select(scene => scene.name).ToList();
            }
        });

    }
#endif
}
