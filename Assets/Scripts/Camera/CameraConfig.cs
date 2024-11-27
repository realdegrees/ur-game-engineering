using UnityEditor.UIElements;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera Config")]
public class CameraConfig : ScriptableObject
{
    [Header("Vertical Snap")]
    [Tooltip("The speed at which the camera moves to the player on the y axis wehn falling in seconds")]
    [Range(0f, 2f)] public float VerticalSnapDuration = 1;
    public AnimationCurve VerticalSnapCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Direction Flip")]
    [Range(0f, 1f)] public float DirectionFlipDuration = .15f;
    public AnimationCurve DirectionFlipCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


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
