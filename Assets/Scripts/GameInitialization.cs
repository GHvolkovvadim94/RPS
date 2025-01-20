using UnityEngine;


public class GameInitialization : MonoBehaviour

{

    public static GameInitialization Instance { get; private set; }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.splashScreen);
        Invoke("InitializeGame", 3.0f);
    }

    private void InitializeGame()
    {
        StartCoroutine(ApiRequests.Instance.ServerStatus());
    }

    public void ValidataUserId()
    {
        if (PlayerPrefs.HasKey("UserId"))
        {
            string userId = PlayerPrefs.GetString("UserId");
            StartCoroutine(ApiRequests.Instance.ValidateUserId(userId));
        }
        else
        {
            ScreenManager.Instance.ShowScreen(ScreenManager.Instance.registrationScreen);
        }
    }
}