using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController instance;

    public enum WeatherType { Rain, Snow }

    [Header("Current Status")]
    public WeatherType currentWeatherType;
    [Range(0, 1f)] public float currentIntensity;

    [Header("Particle Systems")]
    public ParticleSystem rainParticleSystem;
    public ParticleSystem snowParticleSystem;

    [Header("Shader IDs")]
    private int rainShaderID;
    private int snowShaderID;

    [Header("Noise Settings")]
    public float fluctuationSpeed = 0.2f;
    public float minIntensity = 0.3f;
    public float transitionSpeed = 0.5f;

    [Header("Random Logic")]
    public bool isPrecipitating = false; 
    public float minNoWeatherDuration = 60f;
    public float maxNoWeatherDuration = 300f;
    public float minDuration = 60f;
    public float maxDuration = 180f;

    [Header("Snow Accumulation")]
    [Range(0, 1f)] public float snowAmount = 0f; 
    public float fillSpeed = 0.05f;  
    public float meltSpeed = 0.1f;  

    private float _timer;

    private void Awake()
    {
        if (instance == null) instance = this;

        rainShaderID = Shader.PropertyToID("_GlobalRain");
        snowShaderID = Shader.PropertyToID("_GlobalSnow"); 

        SetNextWeatherTime();
    }

    private void Update()
    {
        HandleRandomWeather();
        UpdateVisuals();
        HandleAccumulation();
    }

    private void HandleRandomWeather()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            isPrecipitating = !isPrecipitating;

            if (isPrecipitating)
            {
                currentWeatherType = (Random.value > 0.5f) ? WeatherType.Snow : WeatherType.Rain;

                _timer = Random.Range(minDuration, maxDuration);
            }
            else
            {
                SetNextWeatherTime();
            }
        }

        float targetIntensity = 0f;

        if (isPrecipitating)
        {
            float noise = Mathf.PerlinNoise(Time.time * fluctuationSpeed, 0f);
            targetIntensity = Mathf.Clamp(noise + 0.2f, minIntensity, 1f);
        }

        currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, transitionSpeed * Time.deltaTime);
    }

    private void SetNextWeatherTime()
    {
        _timer = Random.Range(minNoWeatherDuration, maxNoWeatherDuration);
    }

    private void UpdateVisuals()
    {
       
        float rainVal = (currentWeatherType == WeatherType.Rain) ? currentIntensity : 0f;
        float snowVal = (currentWeatherType == WeatherType.Snow) ? currentIntensity : 0f;

   
        Shader.SetGlobalFloat(rainShaderID, rainVal);
        Shader.SetGlobalFloat(snowShaderID, snowVal);

    
        UpdateParticle(rainParticleSystem, rainVal);
        UpdateParticle(snowParticleSystem, snowVal);
    }

    private void UpdateParticle(ParticleSystem ps, float intensity)
    {
        if (ps == null) return;
        var emission = ps.emission;

        if (intensity > 0.05f)
        {
            float maxParticles = (ps == snowParticleSystem) ? 4000f : 2000f;
            emission.rateOverTime = Mathf.Lerp(10, maxParticles, intensity);
        }
        else
        {
            emission.rateOverTime = 0f;
        }
    }

    private void HandleAccumulation()
    {
     
        if (currentWeatherType == WeatherType.Snow && isPrecipitating)
        {
            snowAmount += fillSpeed * Time.deltaTime;
        }

        else if (currentWeatherType == WeatherType.Rain && isPrecipitating)
        {
            snowAmount -= meltSpeed * 2f * Time.deltaTime;
        }

        else
        {
            snowAmount -= meltSpeed * 0.5f * Time.deltaTime;
        }

      
        snowAmount = Mathf.Clamp01(snowAmount);

      
        Shader.SetGlobalFloat("_GlobalSnowLevel", snowAmount);
    }
}