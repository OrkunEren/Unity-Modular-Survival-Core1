using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Mixer Settings")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerSnapshot outdoorSnapshot;
    [SerializeField] private AudioMixerSnapshot indoorSnapshot;

    private float[] _weights = new float[2];
    private AudioMixerSnapshot[] _snapshots = new AudioMixerSnapshot[2];

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource heartbeatSource;
   


    

    private void Awake()
    {
        if (instance != null && instance != this) 
        {
            Destroy(gameObject);
            return;
        }
      
        instance = this;

        _snapshots[0] = outdoorSnapshot;
        _snapshots[1] = indoorSnapshot;


        if (heartbeatSource != null)
        {
            heartbeatSource.loop = true;
            heartbeatSource.volume = 0;
        }

    }

    private void Start()
    {
        if (ShelterDetector.instance != null)
        {
            ShelterDetector.instance.OnShelterChanged += HandleShelterAudio;
        }
    }
    private void HandleShelterAudio(float shelterAmount)
    {    
        _weights[0] = 1.0f - shelterAmount;
     
        _weights[1] = shelterAmount;

        mainMixer.TransitionToSnapshots(_snapshots, _weights, 0f);
    }

    public void PlaySFX(AudioClip clip, float randPitch) 
    {
        if (clip != null) 
        {
            SFXSource.pitch = randPitch; 
            SFXSource.PlayOneShot(clip);
        }
            
    }



    public void SetHeartbeatClip(AudioClip clip) 
    { 
        if(heartbeatSource != null) heartbeatSource.clip = clip;
    }

    public void UpdateHeartbeatVolume(float fadeSpeed, bool shouldPlay)
    {
        if (heartbeatSource == null) return;

        

        if (shouldPlay && !heartbeatSource.isPlaying)
        {
            heartbeatSource.Play();
        }

        float targetVolume = shouldPlay ? 1f : 0f;

        if (Mathf.Abs(heartbeatSource.volume - targetVolume) > 0.01f)
        {
            heartbeatSource.volume = Mathf.MoveTowards(heartbeatSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        }

        if (!shouldPlay && heartbeatSource.volume <= 0.01f && heartbeatSource.isPlaying)
        {
            heartbeatSource.Stop();
        }
    }

    public void PlayMusic(AudioClip clip) 
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume) 
    {
        musicSource.volume = volume;
    }


}
