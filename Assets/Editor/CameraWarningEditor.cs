using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CameraWarningEditor
{
    static CameraWarningEditor()
    {
        EditorApplication.hierarchyChanged += CheckCamerasInScene;
    }

    private static List<Camera> issuedWarnings = new();
    private static void CheckCamerasInScene()
    {
        if (Application.isPlaying) return;

        // Find all cameras in the current scene
        Camera[] allCameras = Object.FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            // Check if the camera is part of the base scene
            if (cam.CompareTag("MainCamera") && !IsPartOfBaseScene(cam) && !issuedWarnings.Contains(cam))
            {
                issuedWarnings.Add(cam);
                Debug.LogWarning($"Camera '{cam.name}' in scene '{cam.gameObject.scene.name}' " +
                                 "will conflict with the Cinemachine camera at runtime when it is additively loaded from the BaseScene!" +
                                 "If you need to modify camera setting use the interface provided by the global CameraManager or adjust the camera in the BaseScene.");

                var iconContent = EditorGUIUtility.IconContent("console.warnicon");
                EditorGUIUtility.SetIconForObject(cam.gameObject, (Texture2D)iconContent.image);
            }
        }
    }

    private static bool IsPartOfBaseScene(Camera cam)
    {
        return cam.gameObject.scene.name == "BaseScene";
    }
}