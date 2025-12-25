using System;
using UnityEngine;

public class ShelterDetector : MonoBehaviour
{
    public static ShelterDetector instance;

    public event Action<float> OnShelterChanged;

    [Header("Settings")]
    [SerializeField] Transform playerTransform;
    [SerializeField] LayerMask roofLayer;
    [SerializeField] float rayDistance;
    [SerializeField] float transitionSpeed;

    [Range(0, 1)] 
    public float shelterAmount;

    public bool isUnderRoof = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        CheckRoof();
    }

    void CheckRoof() 
    {
        if (Physics.Raycast(playerTransform.position + Vector3.up, Vector3.up, out RaycastHit hit, rayDistance, roofLayer))
        {
            isUnderRoof = true;
        }
        else 
        {
            isUnderRoof = false;
        }
        float targetValue = isUnderRoof ? 1f : 0f;

        float previousAmount = shelterAmount;

        shelterAmount = Mathf.MoveTowards(shelterAmount, targetValue, Time.deltaTime * transitionSpeed);

        if (Mathf.Abs(shelterAmount - previousAmount) > 0.001f)
        {
            OnShelterChanged?.Invoke(shelterAmount);
           
        }
        
    }
}
