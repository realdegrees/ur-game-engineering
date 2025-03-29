using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CameraModifierZone : EditorZone<CameraModifierZone>
{
    [Tooltip("This defines which camera type will be used by cinemachine when the player enters the trigger")]
    public CameraType requiredCameraType;
    public Transform target;

    public float zoom = 1; // TODO: implement fov change (with curve)


    private CameraType? cachedType;
    private Transform cachedTarget;


    #region Zone Events

    protected override void Awake()
    {
        base.Awake();
        OnActivate.AddListener(ApplyModifier);
        OnDeactivate.AddListener(ResetCamera);
        OnDurationPassed.AddListener(ResetCamera);
    }
    #endregion

    private void ApplyModifier()
    {
        cachedType = CameraManager.Instance.GetCameraType();
        cachedTarget = CameraManager.Instance.GetTarget();
        CameraManager.Instance.SetCameraType(requiredCameraType, target);
    }

    private void ResetCamera()
    {
        var currentType = CameraManager.Instance.GetCameraType();
        if (currentType == requiredCameraType && cachedType.HasValue)
        {
            CameraManager.Instance.SetCameraType(cachedType.Value, cachedTarget);
            cachedTarget = null;
            cachedType = null;
        }
    }
}
