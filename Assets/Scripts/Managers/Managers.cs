using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers instance;

    private void Awake()
    {
        if (instance != null && instance != this) 
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
