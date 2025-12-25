using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class AmbientSound : MonoBehaviour
{
    public static event Action OnLightningStrike;
    public enum SoundType { Rain, Wind }

    [Header("Settings")]
    public SoundType type;
    public float maxVolume = 1.0f;

    [Header("Pitch Settings")]
    public bool enablePitchShift = true;
    public Vector2 pitchRange = new Vector2(0.8f, 1.2f);

    [Header("Clips")]
    [SerializeField] private AudioClip lowIntensityClip;
    [SerializeField] private AudioClip mediumIntensityClip;
    [SerializeField] private AudioClip highIntensityClip;

    [Header("Thunder Settings ")]
    [SerializeField] private AudioClip[] thunderClips;
    private float thunderTimer;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = true;



        if (!_audioSource.isPlaying) _audioSource.Play();
    }

   

    private void Update()
    {
        if (WeatherController.instance == null) return;

        if (WeatherController.instance.currentWeatherType == WeatherController.WeatherType.Snow)
        {
            _audioSource.volume = Mathf.Lerp(_audioSource.volume, 0f, Time.deltaTime);
            return;
        }
        float intensity = WeatherController.instance.currentIntensity;
        AdjustSound(intensity);
    }

    

    private void AdjustSound(float intensity)
    {
        switch (type)
        {
            case SoundType.Rain:
                HandleRainLogic(intensity);
                break;

            case SoundType.Wind:
                HandleWindLogic(intensity);
                break;
        }
    }

    private void HandleRainLogic(float intensity)
    {
        AudioClip targetClip = lowIntensityClip;

        if (intensity > 0.4f) targetClip = mediumIntensityClip;
        if (intensity > 0.7f) targetClip = highIntensityClip;


        if (_audioSource.clip != targetClip && targetClip != null)
        {
            float currentTime = 0;
            if (_audioSource.clip != null)
            {
                currentTime = _audioSource.time;
            }
            _audioSource.clip = targetClip;

            if (targetClip.length > 0)
                _audioSource.time = currentTime % targetClip.length;

            _audioSource.Play();
        }


        _audioSource.volume = Mathf.Lerp(0f, maxVolume, intensity);


        if (intensity > 0.6f && thunderClips != null && thunderClips.Length > 0)
        {
            thunderTimer -= Time.deltaTime;
            if (thunderTimer <= 0)
            {
                OnLightningStrike?.Invoke();

                if (AudioManager.instance != null)
                {
                    float randPitch = Random.Range(0.8f, 1.2f);
                    int randomClip = Random.Range(0, thunderClips.Length);
                    AudioClip currentThunder = thunderClips[randomClip];
                    AudioManager.instance.PlaySFX(currentThunder, randPitch);
                    StartCoroutine(PlayThunderDelayed(currentThunder, randPitch));
                    thunderTimer = Random.Range(5f, 15f);
                }

        
            }
        }
    }

    IEnumerator PlayThunderDelayed(AudioClip clip , float pitch) 
    {
        float delay = Random.Range(.2f, 1.5f);
        yield return new WaitForSeconds(delay);
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(clip, pitch);
    }

    private void HandleWindLogic(float intensity)
    {

        AudioClip targetClip = lowIntensityClip;
        if (intensity > 0.4f) targetClip = mediumIntensityClip;
        if (intensity > 0.7f) targetClip = highIntensityClip;

        if (_audioSource.clip != targetClip && targetClip != null)
        {
            float currentTime = 0;
            if (_audioSource.clip != null)
            {
                currentTime = _audioSource.time;
            }

            _audioSource.clip = targetClip;

            if (targetClip.length > 0)
                _audioSource.time = currentTime % targetClip.length;

            _audioSource.Play();
        }

        _audioSource.volume = Mathf.Lerp(0.1f, maxVolume, intensity);

        if (enablePitchShift)
        {
            _audioSource.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, intensity);
        }
    }
}