#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class BaseState : ScriptableObject
{
    public float Progress { get; protected set; } = 0;
    public float StartTime { get; protected set; } = 0;
    public bool Active { get; protected set; } = false;

    public bool runParallel = false;

    [Header("Debug")]
    public bool enableStateEventLogs = false;
}

#if UNITY_EDITOR
[CustomEditor(typeof(BaseState), editorForChildClasses: true)]
public class BaseStateEditor : Editor
{
    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }
    public override void OnInspectorGUI()
    {
        BaseState baseState = (BaseState)target;

        // Draw the default inspector
        DrawDefaultInspector();

        // Draw a box with a green background if active
        if (baseState.Active)
        {
            GUIStyle boxStyle = new(GUI.skin.box);
            boxStyle.normal.background = EditorGUIUtility.whiteTexture;
            boxStyle.normal.textColor = Color.green;
            boxStyle.alignment = TextAnchor.MiddleCenter;
            boxStyle.fontStyle = FontStyle.Bold;

            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            GUILayout.Box("State is Active", boxStyle, GUILayout.ExpandWidth(true));

            GUI.backgroundColor = originalColor;
        }
    }
}
#endif