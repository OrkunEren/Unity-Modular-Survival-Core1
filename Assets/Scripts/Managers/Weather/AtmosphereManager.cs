using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AtmosphereManager : MonoBehaviour
{
    [Header("Lighting")]
    public Light directionalLight;
    public float maxLightIntensity = 1.5f;
    public AnimationCurve intensityCurve; 
    public Gradient ambientColor;

    [Header("Fog Settings")]
    public bool fogControl = true;
    public Color dayFogColor;
    public Color nightFogColor;

    [Header("Volume Control")]
    [SerializeField] Volume globalVolume;
    private ColorAdjustments colorAdj;
    private Vignette vignette;

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out colorAdj);
            globalVolume.profile.TryGet(out vignette);
        }
    }

    private void Update()
    {
        float time = TimeManager.instance.currentTime;

        float rainIntensity = WeatherController.instance.currentIntensity;

        UpdateSunAndAmbient(time);
        UpdateFog(time, rainIntensity);
        UpdateVolume(rainIntensity);
    }

    private void UpdateSunAndAmbient(float time)
    {
        if (directionalLight != null)
        {
            float sunDegree = (time * 360) - 90f;
            directionalLight.transform.rotation = Quaternion.Euler(sunDegree, 170f, 0f);

            float rainDimming = Mathf.Lerp(1f, 0.3f, WeatherController.instance.currentIntensity);

            if (intensityCurve != null)
                directionalLight.intensity = intensityCurve.Evaluate(time) * maxLightIntensity * rainDimming;

            if (ambientColor != null)
                RenderSettings.ambientLight = ambientColor.Evaluate(time);
        }
    }

    private void UpdateFog(float time, float intensity)
    {
        if (!fogControl) return;

        var type = WeatherController.instance.currentWeatherType;

        RenderSettings.fog = true;

        float baseDensity = 0.005f + (1 - intensityCurve.Evaluate(time)) * 0.01f;

        float weatherDensityFactor = (type == WeatherController.WeatherType.Snow) ? 0.08f : 0.03f;
        float addedDensity = Mathf.Lerp(0f, weatherDensityFactor, intensity);

        RenderSettings.fogDensity = baseDensity + addedDensity;

        Color currentFogColor = Color.Lerp(nightFogColor, dayFogColor, intensityCurve.Evaluate(time));

        Color targetWeatherColor;
        if (type == WeatherController.WeatherType.Snow)
        {
            targetWeatherColor = Color.white;
        }
        else
        {
            targetWeatherColor = Color.gray;
        }

        RenderSettings.fogColor = Color.Lerp(currentFogColor, targetWeatherColor, intensity);
    }

    private void UpdateVolume(float rainVal)
    {
        if (globalVolume == null) return;

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.2f, 0.45f, rainVal);

        if (colorAdj != null)
        {
            
            colorAdj.saturation.value = Mathf.Lerp(0f, -35f, rainVal);
            colorAdj.postExposure.value = Mathf.Lerp(0f, -0.5f, rainVal); 
            colorAdj.contrast.value = Mathf.Lerp(0f, 15f, rainVal);
        }
    }
}