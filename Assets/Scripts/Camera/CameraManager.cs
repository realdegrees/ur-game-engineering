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

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out CinemachineVirtualCamera virtualCamera)) // Crispy clean
            {
                cameras.Add(virtualCamera);
                activeCamera = virtualCamera;
                framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        player.OnLand.AddListener(() =>
        {
            if (verticalSnapCoroutine != null)
                StopCoroutine(verticalSnapCoroutine);

            verticalSnapCoroutine = StartCoroutine(IncreaseDampening(config.VerticalSnapDuration));
        });
        player.OnStartFaling.AddListener(() =>
        {
            if (verticalSnapCoroutine != null)
                StopCoroutine(verticalSnapCoroutine);

            verticalSnapCoroutine = StartCoroutine(DecreaseDampening(config.VerticalSnapDuration));
        });
    }

    private IEnumerator DecreaseDampening(float duration)
    {
        float elapsedTime = 0;
        float initialDampening = framingTransposer.m_YDamping;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            framingTransposer.m_YDamping = Mathf.Lerp(initialDampening, 0, elapsedTime / duration);
            yield return null;
        }
    }
    private IEnumerator IncreaseDampening(float duration)
    {
        float elapsedTime = 0;
        float initialDampening = framingTransposer.m_YDamping;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            framingTransposer.m_YDamping = Mathf.Lerp(initialDampening, config.VerticalSnapDuration, elapsedTime / duration);
            yield return null;
        }
    }
}
