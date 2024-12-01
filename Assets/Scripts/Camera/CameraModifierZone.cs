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

    #region Gizmos
    // Draw the zone in the editor
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (!showZone) return;

        // Need to get component seperately here because Awake() is not called in editor mode
        if (!TryGetComponent(out Collider2D collider))
            return;

        var bounds = collider.bounds;

        if (target != null)
        {
            Gizmos.color = zoneColor;
            Gizmos.DrawWireCube(target.position, Vector3.one * 0.2f);
            Gizmos.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.2f);
            Gizmos.DrawLine(bounds.center, target.position);
        }

        if (cooldown > 0 && currentCooldown > 0)
        {
            var width = bounds.size.x * (currentCooldown / cooldown);
            var height = bounds.size.y;
            var center = new Vector3(bounds.center.x - (bounds.size.x - width) / 2, bounds.min.y + height / 2, bounds.center.z);

            Gizmos.color = Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawCube(center, new Vector3(width, height, bounds.size.z));
        }
    }


    #endregion

}
