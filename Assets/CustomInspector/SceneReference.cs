using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

[Serializable]
public struct SceneReference
{
    // This field is only used in the editor to drag & drop the scene asset.
#if UNITY_EDITOR
    [SerializeField]
    private SceneAsset sceneAsset;
#endif

    // This field is serialized at runtime. It should store the scene's path.
    [SerializeField]
    private string scenePath;

    // Expose the scene name at runtime.
    public string SceneName
    {
        get
        {
#if UNITY_EDITOR
            // In the editor, update the scenePath from the scene asset if available.
            if (sceneAsset != null)
            {
                scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            }
#endif
            // Return the file name without extension
            return System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }
    }

#if UNITY_EDITOR
    // This method lets the editor update the scenePath automatically when changes occur.
    public void SetSceneAsset(SceneAsset asset)
    {
        sceneAsset = asset;
        scenePath = AssetDatabase.GetAssetPath(asset);
    }
#endif
}
