using UnityEngine;

public class TimeManager : MonoBehaviour
{
   public static TimeManager instance { get; private set; }

    [Header("Day Settings")]
    public float dayLength = 120f;
    [Range(0, 1)] public float currentTime = .25f;
    public int days = 1;

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        CalculateTime();
    }

    private void CalculateTime()
    {
        currentTime += (Time.deltaTime / dayLength);
        if (currentTime >= 1)
        {
            currentTime = 0;
            days++;
        }
    }

}
