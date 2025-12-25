using UnityEngine;

[System.Serializable]
public class GameData 
{
    public float health;
    public float hunger;
    public float thirst;

    public GameData() 
    {
        health = 100f;
        hunger = 100f;
        thirst = 100f;
    }
}
