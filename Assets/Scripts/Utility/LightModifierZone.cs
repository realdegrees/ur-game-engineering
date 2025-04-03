using System.Linq;
using UnityEngine.Rendering.Universal;

public class LightModifierZone : EditorZone<LightModifierZone>
{
    public Light2D globalLight;
    public float lightIntensity = 1f;
    private float defaultIntensity = 1f;

    #region Zone Events

    protected override void Awake()
    {
        globalLight = FindObjectsOfType<Light2D>().Where(light => light.lightType == Light2D.LightType.Global).FirstOrDefault();
        defaultIntensity = globalLight.intensity;
        base.Awake();
        OnActivate.AddListener((go) =>
        {
            globalLight.intensity = lightIntensity;
        });
        OnDeactivate.AddListener(() =>
        {
            globalLight.intensity = defaultIntensity;
        });
    }
    #endregion
}
