using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;
    private float baseIntensity; // The base intensity of the light
    [Range(0f, 1f)] public float flickerIntensity = 0.5f; // The intensity of the flicker effect
    [Range(0f, 1f)] public float flickerSpeed = 0.5f; // The speed of the flicker effect
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out light2D);
        baseIntensity = light2D.intensity; // Store the base intensity of the light

        // Start a coroutine that flickers the light
        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            // Randomly set a target intensity based on flickerIntensity
            float targetIntensity = Random.Range(baseIntensity - flickerIntensity * baseIntensity, baseIntensity + flickerIntensity * baseIntensity);
            float currentIntensity = light2D.intensity;
            float duration = 1 - Random.Range(0, flickerSpeed);
            float elapsedTime = 0f;

            // Gradually change the intensity to the target value
            while (elapsedTime < duration)
            {
                light2D.intensity = Mathf.Lerp(currentIntensity, targetIntensity, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the final intensity is set
            light2D.intensity = targetIntensity;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
