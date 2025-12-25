using System;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;

    public event Action OnStatsChanged;

    [Header("Settings")]
    public float maxHealth = 100f;
    public float maxHunger = 100f;
    public float maxThirst = 100f;

    [Header("Decrease Ratio (per sec)")]
    public float hungerDecay = .5f; // decrease .5f per sec
    public float thirtsDecay = 1.0f;
    public float decreaseHPWhenHungerOrThirst = 1f;

    public float CurrentHealth { get; private set; }
    public float CurrentHunger { get; private set; }
    public float CurrentThirst { get; private set; }
    public float CurrentBodyTemp { get; private set; }
    public float CurrentWetness { get; private set; }

    [Header("SurvivalStats(Temperature)")]
    public float idealBodyTemp;
    public float criticalBodyTemp;
    public float maxWetness = 100f;
    public float minWetness = 0f;

    [Header("Thermal Rates")]
    public float wetnessIncreaseRate = 5f;
    public float dryingRate = 1f;
    public float warmingRate = 1f;
    public float freezingRate = .5f;
    public float hypothermiaDamage = 2f;

    [Header("Heat Source Info")]
    public bool isNearFire = false; 
    public float fireDryingRate = 10f; 
    public float fireWarmingRate = 5f;

    [Header("UI")]
    public Slider healthBar;
    public Slider hungerBar;
    public Slider thirstBar;
    public Slider tempBar;
    public Slider wetnessBar;
    public float lerpSpeed;

    private bool wasCritical = false;
    private float criticalThreshold = 30f;

    private HeatSource heatSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            instance = this;

   

        CurrentHealth = maxHealth;
        CurrentHunger = maxHunger;
        CurrentThirst = maxThirst;
        CurrentBodyTemp = idealBodyTemp;
        CurrentWetness = 0;
    }

   
 



    private void Update()
    {
        HandleSurvivalStats();
        HandleBasicStats();

        bool isCritical = CurrentHealth <= criticalThreshold;

        if (isCritical != wasCritical)
        {
            OnStatsChanged?.Invoke();

            wasCritical = isCritical;
        }

        UpdateUI();

    }

    private void HandleBasicStats()
    {
        if (CurrentHunger > 0)
            CurrentHunger -= hungerDecay * Time.deltaTime;

        if (CurrentThirst > 0)
            CurrentThirst -= thirtsDecay * Time.deltaTime;

        if (CurrentThirst <= 0 || CurrentHunger <= 0)
        {
            CurrentHealth -= decreaseHPWhenHungerOrThirst * Time.deltaTime;

            if (CurrentHealth <= 0)
            {
                CheckDeath(); 
            }

        }
    }

    void HandleSurvivalStats()
    {
        bool isRaining = false;

        float CurrentIntensity =WeatherController.instance.currentIntensity;


        if (WeatherController.instance != null && WeatherController.instance.currentIntensity > .1f)
            isRaining = true;


        bool isUnderRoof = false;

        if (ShelterDetector.instance != null && ShelterDetector.instance.isUnderRoof )
            isUnderRoof = true;
       
        bool isWetting = false;

        if(!isUnderRoof && isRaining )
            isWetting=true;

        if (isNearFire)
        {
            CurrentWetness = Mathf.MoveTowards(CurrentWetness, minWetness, Time.deltaTime * fireDryingRate);
        }
        else if (isWetting)
        {
            CurrentWetness = Mathf.MoveTowards(CurrentWetness, maxWetness, Time.deltaTime * CurrentIntensity * wetnessIncreaseRate);
        }
        else
        {
            CurrentWetness = Mathf.MoveTowards(CurrentWetness, minWetness, Time.deltaTime * dryingRate);
        }



        // Temperature Settings

        float targetTemp = idealBodyTemp - (CurrentWetness / 100f) * 5f;

        if (isNearFire)
            targetTemp = 40f;
            
        if (CurrentBodyTemp > targetTemp)
        {           
            CurrentBodyTemp -= freezingRate * Time.deltaTime;
        }
        else if (CurrentBodyTemp < targetTemp)
        {
            float currentWarmingRate = isNearFire ? fireWarmingRate : warmingRate;
            CurrentBodyTemp += currentWarmingRate * Time.deltaTime;
        }       
        if (CurrentBodyTemp < criticalBodyTemp)
        {
            CurrentHealth -= hypothermiaDamage * Time.deltaTime;
            CheckDeath();
        }

    }

    void CheckDeath()
    {
        if (CurrentHealth <= 0)
        {       
            Debug.Log("Player Died!");
        }
    }

    public void LoadState(float loadedHealth, float loadedHunger, float loadedThirst)
    {
        CurrentHealth = loadedHealth;
        CurrentHunger = loadedHunger;
        CurrentThirst = loadedThirst;


        UpdateUI();

        OnStatsChanged?.Invoke();
    }


    public void ChangeStat(ConsumableType type, float amount)
    {
        switch (type)
        {
            case ConsumableType.Health:
                CurrentHealth += amount;
                break;
            case ConsumableType.Thirst:
                CurrentThirst += amount;
                break;
            case ConsumableType.Hunger:
                CurrentHunger += amount;
                break;
            case ConsumableType.Temperature:
                CurrentBodyTemp += amount;
                break;

        }
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        CurrentThirst = Mathf.Clamp(CurrentThirst, 0, maxThirst);
        CurrentHunger = Mathf.Clamp(CurrentHunger, 0, maxHunger);
        CurrentBodyTemp = Mathf.Clamp(CurrentBodyTemp, 30f, 40f);


        OnStatsChanged?.Invoke();

    }


    void UpdateUI()
    {
        if (healthBar != null) healthBar.value = Mathf.Lerp(healthBar.value, CurrentHealth / maxHealth, lerpSpeed * Time.deltaTime);
        if (hungerBar != null) hungerBar.value = Mathf.Lerp(hungerBar.value, CurrentHunger / maxHunger, lerpSpeed * Time.deltaTime);
        if (thirstBar != null) thirstBar.value = Mathf.Lerp(thirstBar.value, CurrentThirst / maxThirst, lerpSpeed * Time.deltaTime);
        
        if (tempBar != null)
        {          
            float tempRatio = Mathf.InverseLerp(30f, 40f, CurrentBodyTemp);
            tempBar.value = Mathf.Lerp(tempBar.value, tempRatio, lerpSpeed * Time.deltaTime);
        }

        if (wetnessBar != null)
        {
            wetnessBar.value = Mathf.Lerp(wetnessBar.value, CurrentWetness / maxWetness, lerpSpeed * Time.deltaTime);
        }
    }


}
