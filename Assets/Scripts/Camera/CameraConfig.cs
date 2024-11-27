using UnityEngine;

[CreateAssetMenu(menuName = "Camera Config")]
public class CameraConfig : ScriptableObject
{
    [Header("Move")]
    [Tooltip("The speed at which the camera moves to the player on the y axis wehn falling in seconds")]
    [Range(0f, 2f)] public float VerticalSnapDuration = 1;

    // Create public get, private set properties for values that are not exposed in editor and are calculated 

    private void OnValidate()
    {
        CalculateValues();
    }
    private void OnEnable()
    {
        CalculateValues();
    }
    private void CalculateValues()
    {
        // Calculate values that re not exposed in editor
    }
}
