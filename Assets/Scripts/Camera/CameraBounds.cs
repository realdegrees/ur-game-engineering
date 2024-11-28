using Cinemachine;
using UnityEngine;


public enum CameraBoundsActivationType
{
    Awake,
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

    protected override void Awake()
    {
        base.Awake();
        CheckRequirements();
        InitListeners();
    }

    private void CheckRequirements()
    {
        if (boundsCollider == GetComponentInChildren<CompositeCollider2D>())
        {
            Debug.LogWarning($"Camera bounds collider on {gameObject.name} has the same root as an editor zone. This will result in the zone triggering when the player collides with the bounds collider! Put the bounds collider on a separate object.");
        }

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

        if (activationType == CameraBoundsActivationType.Awake)
        {
            SetBounds();
        }
    }

    private void InitListeners()
    {
        OnActivate.AddListener(() =>
        {
            if (activationType == CameraBoundsActivationType.Toggle)
            {
                if (GetBounds() == boundsCollider)
                {
                    ResetBounds();
                }
                else
                {
                    SetBounds();
                }
            }
            else if (activationType == CameraBoundsActivationType.Enter)
            {
                SetBounds();
            }
            else if (activationType == CameraBoundsActivationType.Stay)
            {
                SetBounds();
            }
        });
        OnDeactivate.AddListener(() =>
        {
            if (activationType == CameraBoundsActivationType.Stay)
            {
                ResetBounds();
            }
        });
        OnCooldownReset.AddListener(ResetBounds);
    }

    private Collider2D GetBounds()
    {
        return CameraManager.Instance.GetCameraBounds(CameraType.Default);
    }
    private void SetBounds()
    {
        cachedBounds = GetBounds() as CompositeCollider2D;
        CameraManager.Instance.SetCameraBounds(boundsCollider, CameraType.Default);
    }
    private void ResetBounds()
    {
        var currentCameraBounds = GetBounds();
        if (currentCameraBounds == boundsCollider)
        {
            CameraManager.Instance.SetCameraBounds(cachedBounds, CameraType.Default);
            cachedBounds = null;
        }
    }
}
