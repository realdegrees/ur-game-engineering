using System;
using System.Collections;
using Cinemachine;
using Manager;
using UnityEngine;

public enum CameraType
{
    Follow,
    Fixed
}
[Serializable]
public class CameraDetails
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
    private CameraDetails[] cameraDetails;
    private CameraDetails activeCameraDetails;


    private Coroutine verticalSnapCoroutine;
    private Coroutine flipCoroutine;

    public void SetPlayer(PlayerController player)
    {
        if (this.player != null)
        {
            this.player.OnLand.RemoveListener(OnPlayerLand);
            this.player.OnStartFalling.RemoveListener(OnPlayerStartFalling);
            this.player.OnFlip.RemoveListener(OnPlayerFlip);
        }

        player.OnLand.AddListener(OnPlayerLand);
        player.OnStartFalling.AddListener(OnPlayerStartFalling);
        player.OnFlip.AddListener(OnPlayerFlip);

        this.player = player;
        activeCameraDetails.camera.Follow = player.transform;
    }

    private void OnPlayerFlip(bool isFacingRight)
    {
        var isFollowCamera = activeCameraDetails.type == CameraType.Follow;
        if (!isFollowCamera)
            return;

        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        flipCoroutine = StartCoroutine(FlipOffset(config.DirectionFlipDuration));
    }

    private void OnPlayerStartFalling()
    {
        var isFollowCamera = activeCameraDetails.type == CameraType.Follow;
        if (!isFollowCamera)
            return;

        if (verticalSnapCoroutine != null)
            StopCoroutine(verticalSnapCoroutine);

        verticalSnapCoroutine = StartCoroutine(DecreaseDampening(config.VerticalSnapDuration));
    }

    private void OnPlayerLand()
    {
        var isFollowCamera = activeCameraDetails.type == CameraType.Follow;
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
        foreach (CameraDetails details in cameraDetails)
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
    }

    public void SetCameraType(CameraType type, Transform target = null)
    {
        CameraDetails match = null;
        foreach (var details in cameraDetails)
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
        return activeCameraDetails.type;
    }
    public Transform GetTarget()
    {
        return activeCameraDetails.camera.Follow;
    }

    private void SetActiveCamera(CameraDetails details, Transform target = null)
    {
        if (activeCameraDetails != null)
            activeCameraDetails.camera.enabled = false;

        activeCameraDetails = details;
        details.camera.enabled = true;

        if (target != null)
            details.camera.Follow = target;
    }

    private IEnumerator FlipOffset(float duration)
    {
        float elapsedTime = 0;
        var transposer = activeCameraDetails.transposer;
        Debug.Log("DecreaseDampening");
        transposer.m_TrackedObjectOffset.x = -transposer.m_TrackedObjectOffset.x;
        var startValue = transposer.m_TrackedObjectOffset.x;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;
            ratio = config.DirectionFlipCurve.Evaluate(ratio);
            transposer.m_TrackedObjectOffset.x = Mathf.Lerp(startValue, activeCameraDetails.defaultXOffset, ratio);
            yield return null;
        }
    }
    private IEnumerator DecreaseDampening(float duration)
    {
        float elapsedTime = 0;
        Debug.Log("DecreaseDampening");
        var transposer = activeCameraDetails.transposer;
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
        Debug.Log("IncreaseDampening");
        float elapsedTime = 0;
        var transposer = activeCameraDetails.transposer;
        float startValue = transposer.m_YDamping;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;
            ratio = config.VerticalSnapCurve.Evaluate(ratio);
            transposer.m_YDamping = Mathf.Lerp(startValue, activeCameraDetails.defaultDampening, ratio);
            yield return null;
        }
    }
}
