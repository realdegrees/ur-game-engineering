using UnityEngine;


public enum CameraBoundsActivationType
{
    Start,
    Enter,
    Stay,
    Toggle,
}

public class CameraBounds : EditorZone<CameraBounds>
{
    [Header("Camera Bounds Settings")]
    [Tooltip("Defines what triggers the activation of the camera bounds")]
    public CameraBoundsActivationType activationType;

    public CompositeCollider2D boundsCollider;
    private CompositeCollider2D cachedBounds;

    protected override void Start()
    {
        base.Start();
        CheckRequirements();
        InitListeners();
    }

    private void CheckRequirements()
    {
        if (!boundsCollider.isTrigger)
        {
            Debug.LogWarning($"Camera bounds collider on {gameObject.name} is not set to 'Trigger'. Setting it automatically.");
            boundsCollider.isTrigger = true;
        }

        if (boundsCollider.geometryType != CompositeCollider2D.GeometryType.Polygons)
        {
            Debug.LogWarning($"Camera bounds collider on {gameObject.name} is not set to 'GeometryType.Polygon'. Setting it automatically.");
            boundsCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        }

        if (activationType == CameraBoundsActivationType.Start)
        {
            SetBounds();
        }
    }

    private void InitListeners()
    {
        OnActivate.AddListener((go) =>
        {
            switch (activationType)
            {
                case CameraBoundsActivationType.Start:
                    break;
                case CameraBoundsActivationType.Enter:
                    SetBounds();
                    break;
                case CameraBoundsActivationType.Stay:
                    SetBounds();
                    break;
                case CameraBoundsActivationType.Toggle:
                    if (GetBounds() == boundsCollider)
                        ResetBounds();
                    else
                        SetBounds();
                    break;
                default:
                    break;
            }

        });
        OnDeactivate.AddListener(() =>
        {
            ResetBounds();
        });
    }

    private Collider2D GetBounds() => CameraManager.Instance.GetCameraBounds(CameraType.Follow);

    private void SetBounds()
    {
        cachedBounds = GetBounds() as CompositeCollider2D;
        CameraManager.Instance.SetCameraBounds(boundsCollider, CameraType.Follow);
    }
    private void ResetBounds()
    {
        var currentCameraBounds = GetBounds();
        if (currentCameraBounds == boundsCollider)
        {
            CameraManager.Instance.SetCameraBounds(cachedBounds, CameraType.Follow);
            cachedBounds = null;
        }
    }
}
