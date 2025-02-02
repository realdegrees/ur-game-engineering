using System;
using System.Collections;
using Cinemachine;
using Manager;
using UnityEngine;

public enum CameraType
{
    Default,
    Fixed
}
[Serializable]
public class CameraData
{
    public CameraType type;
    public CinemachineVirtualCamera camera;
    [HideInInspector]
    public CinemachineFramingTransposer transposer;
    [HideInInspector]
    public float defaultDampening;
    [HideInInspector]
    public float defaultXOffset;
}

public class CameraManager : Manager<CameraManager>
{
    public PlayerController player;
    public CameraConfig config;
    [SerializeField]
    private CameraData[] cameraData;
    private CameraData activeCameraData;


    private Coroutine verticalSnapCoroutine;
    private Coroutine flipCoroutine;


    public void SetCameraBounds(Collider2D bounds, CameraType cameraType)
    {
        var camera = GetCameraData(cameraType).camera;
        if (camera == null) return;

        CinemachineConfiner2D confiner = camera.GetComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = bounds;
    }
    public Collider2D GetCameraBounds(CameraType cameraType)
    {
        var camera = GetCameraData(cameraType).camera;
        if (camera != null && camera.TryGetComponent(out CinemachineConfiner2D confiner))
        {
            return confiner.m_BoundingShape2D;
        }
        return null;
    }


    private void OnPlayerFlip(bool isFacingRight)
    {
        var isFollowCamera = activeCameraData.type == CameraType.Default;
        if (!isFollowCamera)
            return;

        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        flipCoroutine = StartCoroutine(FlipOffset(config.DirectionFlipDuration));
    }


    private void OnPlayerStartFalling()
    {
        var isFollowCamera = activeCameraData.type == CameraType.Default;
        if (!isFollowCamera)
            return;

        if (verticalSnapCoroutine != null)
            StopCoroutine(verticalSnapCoroutine);

        verticalSnapCoroutine = StartCoroutine(DecreaseDampening(config.VerticalSnapDuration));
    }

    private void OnPlayerLand()
    {
        var isFollowCamera = activeCameraData.type == CameraType.Default;
        if (!isFollowCamera)
            return;

        if (verticalSnapCoroutine != null)
            StopCoroutine(verticalSnapCoroutine);

        verticalSnapCoroutine = StartCoroutine(IncreaseDampening(config.VerticalSnapDuration));
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        foreach (CameraData details in cameraData)
        {
            // set transposer
            details.transposer = details.camera.GetCinemachineComponent<CinemachineFramingTransposer>();
            // Initialize default values
            details.defaultDampening = details.transposer.m_YDamping;
            details.defaultXOffset = details.transposer.m_TrackedObjectOffset.x;

            // set active camera
            var isEnabled = details.camera.enabled;
            if (isEnabled)
            {
                SetActiveCamera(details);
            }
        }

        // ! Important to do this after setting up the cameras
        player.OnLand.AddListener(OnPlayerLand);
        player.OnStartFalling.AddListener(OnPlayerStartFalling);
        player.OnFlip.AddListener(OnPlayerFlip);
    }

    public void SetCameraType(CameraType type, Transform target = null)
    {
        CameraData match = null;
        foreach (var details in cameraData)
        {
            if (details.type == type)
            {
                match = details;
                break;
            }
        }

        if (match != null)
        {
            SetActiveCamera(match, target);
        }
    }
    public CameraType GetCameraType()
    {
        return activeCameraData.type;
    }
    public CameraData GetCameraData(CameraType type)
    {
        foreach (var details in cameraData)
        {
            if (details.type == type)
            {
                return details;
            }
        }
        return null;
    }

    public Transform GetTarget()
    {
        return activeCameraData.camera.Follow;
    }

    private void SetActiveCamera(CameraData details, Transform target = null)
    {
        details.camera.enabled = true;

        if (activeCameraData?.camera != null && activeCameraData.camera != details.camera)
            activeCameraData.camera.enabled = false;

        activeCameraData = details;

        if (target != null)
            details.camera.Follow = target;
    }

    private IEnumerator FlipOffset(float duration)
    {
        float elapsedTime = 0;
        var transposer = activeCameraData.transposer;
        transposer.m_TrackedObjectOffset.x = -transposer.m_TrackedObjectOffset.x;
        var startValue = transposer.m_TrackedObjectOffset.x;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;
            ratio = config.DirectionFlipCurve.Evaluate(ratio);
            transposer.m_TrackedObjectOffset.x = Mathf.Lerp(startValue, activeCameraData.defaultXOffset, ratio);
            yield return null;
        }
    }
    private IEnumerator DecreaseDampening(float duration)
    {
        float elapsedTime = 0;
        var transposer = activeCameraData.transposer;
        float startValue = transposer.m_YDamping;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;
            ratio = config.VerticalSnapCurve.Evaluate(ratio);
            transposer.m_YDamping = Mathf.Lerp(startValue, 0, ratio);
            yield return null;
        }
    }
    private IEnumerator IncreaseDampening(float duration)
    {
        float elapsedTime = 0;
        var transposer = activeCameraData.transposer;
        float startValue = transposer.m_YDamping;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;
            ratio = config.VerticalSnapCurve.Evaluate(ratio);
            transposer.m_YDamping = Mathf.Lerp(startValue, activeCameraData.defaultDampening, ratio);
            yield return null;
        }
    }
}
