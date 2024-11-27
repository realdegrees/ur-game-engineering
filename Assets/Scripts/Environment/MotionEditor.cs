#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Motion))]
public class MotionEditor : Editor
{
    private Motion motion;

    private void OnEnable()
    {
        motion = (Motion)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Toggle button for waypoint editing
        if (GUILayout.Button(motion.isEditingWaypoints ? "Stop Editing Waypoints" : "Start Editing Waypoints"))
        {
            Undo.RecordObject(motion, "Toggle Waypoint Editing");
            motion.isEditingWaypoints = !motion.isEditingWaypoints;
        }

        // Button to clear the path
        if (GUILayout.Button("Clear Path"))
        {
            Undo.RecordObject(motion, "Clear Path");
            motion.waypoints.Clear();
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        if (!motion.isEditingWaypoints) return;

        Event e = Event.current;

        // Add waypoints with left-click
        if (e.type == EventType.MouseDown && e.button == 0) // Left click
        {
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Undo.RecordObject(motion, "Add Waypoint");
            motion.waypoints.Add(mousePos - (Vector2)motion.transform.position);
            SceneView.RepaintAll();
            e.Use();
        }

        // Draw path in Scene view
        if (motion.waypoints.Count > 1)
        {
            Handles.color = Color.blue;
            for (int i = 0; i < motion.waypoints.Count - 1; i++)
            {
                Handles.DrawLine(
                    motion.transform.position + (Vector3)motion.waypoints[i],
                    motion.transform.position + (Vector3)motion.waypoints[i + 1]);
            }
        }

        // Draw handles for existing points
        for (int i = 0; i < motion.waypoints.Count; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPoint = Handles.PositionHandle(
                motion.transform.position + (Vector3)motion.waypoints[i],
                Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(motion, "Move Waypoint");
                motion.waypoints[i] = newPoint - motion.transform.position;
            }
        }
    }
}

#endif