using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Manager;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : Manager<CameraManager>
{
    public PlayerController player;
    public CameraConfig config;
    private readonly List<CinemachineVirtualCamera> cameras = new();
    private CinemachineVirtualCamera activeCamera;
    private CinemachineFramingTransposer framingTransposer;

    private Coroutine verticalSnapCoroutine;
    private Coroutine flipCoroutine;

    private float defaultDampening;
    private float defaultXOffset;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out CinemachineVirtualCamera virtualCamera)) // Crispy clean
            {
                cameras.Add(virtualCamera);
                var isEnabled = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().enabled;
                if (isEnabled)
                {
                    SetActiveCamera(virtualCamera);
                }

            }
        }

        player.OnLand.AddListener(() =>
        {
            if (verticalSnapCoroutine != null)
                StopCoroutine(verticalSnapCoroutine);

            verticalSnapCoroutine = StartCoroutine(IncreaseDampening(config.VerticalSnapDuration));
        });
        player.OnStartFalling.AddListener(() =>
        {
            if (verticalSnapCoroutine != null)
                StopCoroutine(verticalSnapCoroutine);

            verticalSnapCoroutine = StartCoroutine(DecreaseDampening(config.VerticalSnapDuration));
        });
        player.OnFlip.AddListener((_) =>
        {
            if (flipCoroutine != null)
                StopCoroutine(flipCoroutine);

            flipCoroutine = StartCoroutine(FlipOffset(config.DirectionFlipDuration));
        });
    }

    private void SetActiveCamera(CinemachineVirtualCamera camera)
    {
        if (framingTransposer != null)
            framingTransposer.enabled = false;


        activeCamera = camera;
        framingTransposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.enabled = true;

        // Initialize default values
        defaultDampening = framingTransposer.m_YDamping;
        defaultXOffset = framingTransposer.m_TrackedObjectOffset.x;
    }

    private IEnumerator FlipOffset(float duration)
    {
        float elapsedTime = 0;
        Debug.Log("DecreaseDampening");
        framingTransposer.m_TrackedObjectOffset.x = -framingTransposer.m_TrackedObjectOffset.x;
        var startValue = framingTransposer.m_TrackedObjectOffset.x;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;
            ratio = config.DirectionFlipCurve.Evaluate(ratio);
            framingTransposer.m_TrackedObjectOffset.x = Mathf.Lerp(startValue, defaultXOffset, ratio);
            yield return null;
        }
    }
    private IEnumerator DecreaseDampening(float duration)
    {
        float elapsedTime = 0;
        Debug.Log("DecreaseDampening");
        float startValue = framingTransposer.m_YDamping;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;
            ratio = config.VerticalSnapCurve.Evaluate(ratio);
            framingTransposer.m_YDamping = Mathf.Lerp(startValue, 0, ratio);
            yield return null;
        }
    }
    private IEnumerator IncreaseDampening(float duration)
    {
        Debug.Log("IncreaseDampening");
        float elapsedTime = 0;
        float startValue = framingTransposer.m_YDamping;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / duration;
            ratio = config.VerticalSnapCurve.Evaluate(ratio);
            framingTransposer.m_YDamping = Mathf.Lerp(startValue, defaultDampening, ratio);
            yield return null;
        }
    }
}
