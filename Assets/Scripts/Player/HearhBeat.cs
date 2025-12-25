using System.Diagnostics;
using UnityEngine;


public class HearhBeat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float triggerThreshold = 30f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] AudioClip clip;

    private bool shouldPlay = false;
   
    private void Start()
    {
        if (StatsManager.instance != null)
            StatsManager.instance.OnStatsChanged += EvaluateHealth;
        AudioManager.instance.SetHeartbeatClip(clip);
    }

    private void OnDestroy()
    {
        if (StatsManager.instance != null)
            StatsManager.instance.OnStatsChanged -= EvaluateHealth;
    }

    void EvaluateHealth()
    {
        float currentHealth = StatsManager.instance.CurrentHealth;
        shouldPlay = currentHealth <= triggerThreshold;

    }


    private void Update()
    {
        AudioManager.instance.UpdateHeartbeatVolume( fadeSpeed, shouldPlay);
    }

}
