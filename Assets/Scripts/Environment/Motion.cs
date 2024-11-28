using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionWaypoint
{

}

[ExecuteInEditMode]
public class Motion : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 1f;
    public bool useTrigger = false;
    public Collider2D triggerZone;
    public bool loop = false;

    [HideInInspector]
    public List<Vector2> waypoints = new List<Vector2>();
    public bool isEditingWaypoints = false; // Flag to toggle editing mode

    private void OnDrawGizmos()
    {
        // Draw path in the editor when selected
        if (waypoints.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Gizmos.DrawLine(transform.position + (Vector3)waypoints[i], transform.position + (Vector3)waypoints[i + 1]);
            }
        }

        // Draw points
        Gizmos.color = Color.red;
        foreach (var point in waypoints)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)point, 0.1f);
        }
    }

}
