using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ApiRequests : MonoBehaviour
{
    public static ApiRequests Instance { get; private set; }

    public delegate void UserRegisteredHandler();
    public event UserRegisteredHandler OnUserRegistered;

    public delegate void UserUpdatedHandler();
    public event UserUpdatedHandler OnUserUpdated;

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

    public IEnumerator RegisterRequest(string name)
    {
        string url = "http://localhost:5500/user/register";
        WWWForm form = new WWWForm();
        form.AddField("name", name);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);
                PlayerPrefs.SetString("UserId", response.id);
                PlayerPrefs.SetString("UserName", name);
                OnUserRegistered?.Invoke();
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                UIManager.Instance.ShowError("Error: " + request.error);
            }
        }
    }

    public IEnumerator UpdateRequest(string userId, string name)
    {
        string url = $"http://localhost:5500/user/update/{userId}";
        WWWForm form = new WWWForm();
        form.AddField("newName", name);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerPrefs.SetString("UserName", name);
                OnUserUpdated?.Invoke();
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                UIManager.Instance.ShowError("Error: " + request.error, true);
            }
        }
    }

    public IEnumerator ValidateUserId(string userId)
    {
        string url = $"http://localhost:5500/user/validate/{userId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // User ID is valid
                UIManager.Instance.ShowLobbyScreen();
            }
            else
            {
                // User ID is invalid
                Debug.LogError("Error: " + request.error);
                UIManager.Instance.ShowErrorScreen("Invalid User ID");
            }
        }
    }

    public IEnumerator GetUserName(string userId)
    {
        string url = $"http://localhost:5500/user/name/{userId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<UserNameResponse>(request.downloadHandler.text);
                PlayerPrefs.SetString("UserName", response.name);
                UIManager.Instance.UpdateUserNameText();
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                UIManager.Instance.ShowError("Error: " + request.error);
            }
        }
    }

    public IEnumerator BattleRequest(string userId, Choice playerChoice)
    {
        string url = "http://localhost:5500/match/battle";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("choice", playerChoice.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the result from the server
                RoundResult roundResult = ParseRoundResult(request.downloadHandler.text);
                UIManager.Instance.ShowResultScreen(roundResult);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                UIManager.Instance.ShowError("Error: " + request.error);
            }
        }
    }

    private RoundResult ParseRoundResult(string result)
    {
        // Implement parsing logic based on server response
        // For example:
        if (result.Contains("win"))
        {
            return RoundResult.Win;
        }
        else if (result.Contains("lose"))
        {
            return RoundResult.Lose;
        }
        else
        {
            return RoundResult.Draw;
        }
    }
}

[System.Serializable]
public class RegisterResponse
{
    public string id;
}

[System.Serializable]
public class UserNameResponse
{
    public string name;
}