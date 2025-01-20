// GameInitialization.cs
using UnityEngine;

public class GameInitialization : MonoBehaviour
{
    private void Start()
    {
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.splashScreen);
        Invoke("InitializeGame", 3.0f); // Показать сплэш-скрин в течение 3 секунд
    }

    private void InitializeGame()
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