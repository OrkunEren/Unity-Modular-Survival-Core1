using UnityEngine;
using System.IO;
public class SaveManager : MonoBehaviour
{
   public static SaveManager instance;

    private string saveFileName = "game_save.json";
    private string saveFilePath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        if (StatsManager.instance != null)
        {
            data.health = StatsManager.instance.CurrentHealth;
            data.hunger = StatsManager.instance.CurrentHunger;
            data.thirst = StatsManager.instance.CurrentThirst;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved To  > " + saveFilePath);

    }

    public void LoadGame() 
    {
        if (!File.Exists(saveFilePath)) return;

        string json = File.ReadAllText(saveFilePath);

        GameData data = JsonUtility.FromJson<GameData>(json);

        if (StatsManager.instance != null)
        {
            StatsManager.instance.LoadState(data.health, data.hunger, data.thirst);
        }

        Debug.Log("Game Loaded");


    }

}
