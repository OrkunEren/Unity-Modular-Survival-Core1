using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Music Settings")]
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip battleMusic;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
    }

    private void Start()
    {
        AudioManager.instance.PlayMusic(menuMusic);
    }

   

}
