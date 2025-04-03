using System.Linq;
using UnityEngine.Rendering.Universal;

using System.Collections;
using UnityEngine;

public class LightModifierZone : EditorZone<LightModifierZone>
{
    public Light2D globalLight;
    public float lightIntensity = 1f;
    private float defaultIntensity = 1f;
    private Color defaultBackgroundColor = Color.white;
    public float transitionDuration = .6f;
    private Coroutine transitionCoroutine;

    #region Zone Events

    protected override void Start()
    {
        base.Start();
        defaultBackgroundColor = Camera.main.backgroundColor;
        globalLight = FindObjectsOfType<Light2D>().Where(light => light.lightType == Light2D.LightType.Global).FirstOrDefault();
        defaultIntensity = globalLight.intensity;
        OnActivate.AddListener((go) =>
        {
            float weight = lightIntensity / defaultIntensity;
            StartTransition(globalLight.intensity, lightIntensity, weight);
        });
        OnDeactivate.AddListener(() =>
        {
            float weight = defaultIntensity / lightIntensity;
            StartTransition(globalLight.intensity, defaultIntensity, weight);
        });
    }

    private void StartTransition(float from, float to, float weight)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionLightIntensity(from, to, weight, transitionDuration));
    }

    private IEnumerator TransitionLightIntensity(float from, float to, float weight, float duration)
    {
        float elapsed = 0f;
        Color startColor = Camera.main.backgroundColor;
        Color targetColor = Color.Lerp(defaultBackgroundColor, Color.black, 1f - weight);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            globalLight.intensity = Mathf.Lerp(from, to, t);
            Camera.main.backgroundColor = Color.Lerp(startColor, targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        globalLight.intensity = to;
        Camera.main.backgroundColor = targetColor;
    }

    #endregion
}
