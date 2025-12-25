using System;
using UnityEngine;

public class HeatSource : MonoBehaviour
{
    


    [SerializeField] GameObject fireVisuals;
    private bool isLit= false;


    private void Start()
    {
        
    }

    void ToggleFire() 
    {
        isLit = !isLit;
        UpdateFireState();
    }
    void UpdateFireState()
    {
        if (fireVisuals != null)
        {
            fireVisuals.SetActive(isLit);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
            return;
        if(StatsManager.instance == null)
            return;

        
        StatsManager.instance.isNearFire = true;

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (StatsManager.instance == null)
            return;

        StatsManager.instance.isNearFire = false;
    }

   
}
