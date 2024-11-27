using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class CameraModifierZone : MonoBehaviour
{
    public CameraType modifierType;
    public Transform target;
    [Range(1, 3)]
    public int numberOfAllowedActivations = 1;
    private CameraType? cachedType;
    private Transform cachedTarget;
    [Range(0, 60)]
    [Description("The duration of the camera modifier in seconds (0 = until collision ends)")]
    public float duration = 0;
    public UnityEvent onActivate = new();
    public UnityEvent onDeactivate = new();

    private Coroutine resetCoroutine;

    // Set the camera type to the modifier type when the player enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (numberOfAllowedActivations == 0) return;
        if (other.transform.root.TryGetComponent(out PlayerController controller)) // ! re-use snippet
        {
            if (other == controller.interactionCollider)
            {
                numberOfAllowedActivations--;
                resetCoroutine = StartCoroutine(ResetAfterDuration());
                ApplyModifier();
            }

        }
    }

    private IEnumerator ResetAfterDuration()
    {
        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
            ResetCamera();
        }
    }

    private void ApplyModifier()
    {
        cachedType = CameraManager.Instance.GetCameraType();
        cachedTarget = CameraManager.Instance.GetTarget();
        CameraManager.Instance.SetCameraType(modifierType, target);
        onActivate.Invoke();
    }

    // Reset the camera type to the cached type when the player exits the trigger but only if the camera type hasn't been altered
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.TryGetComponent(out PlayerController controller)) // ! re-use snippet
        {
            if (other == controller.interactionCollider)
            {
                ResetCamera();
            }
        }
    }

    private void ResetCamera()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }


        var currentType = CameraManager.Instance.GetCameraType();
        if (currentType == modifierType)
        {
            CameraManager.Instance.SetCameraType(cachedType.Value, cachedTarget);
            cachedTarget = null;
            cachedType = null;
            onActivate.Invoke();
        }
    }
}
