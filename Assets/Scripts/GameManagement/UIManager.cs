using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Registration Screen Elements")]
    public TMP_InputField nameInputField;
    public TextMeshProUGUI errorText;
    public Button confirmButton;

    [Header("Name Change Screen Elements")]
    public TMP_InputField nameChangeInputField;
    public TextMeshProUGUI nameChangeErrorText;
    public Button nameChangeConfirmButton;
    public Button nameChangeCloseButton;

    [Header("Lobby Screen Elements")]
    public Button startButton;
    public TextMeshProUGUI userStatsText;
    public TextMeshProUGUI userNameText;
    public Button changeNameButton;

    [Header("Global UI Elements")]
    public TextMeshProUGUI userIdText;

    [Header("Loading Screen Elements")]
    public Slider loadingSlider;
    public Button continueButton;

    [Header("Queue Screen Elements")]
    public Button cancelButton;
    public TextMeshProUGUI queueUserNameText;
    public TextMeshProUGUI queueUserStatsText;
    public TextMeshProUGUI opponentNameText;
    public TextMeshProUGUI opponentStatsText;
    public TextMeshProUGUI countdownText;

    [Header("Error Screen Elements")]
    public TextMeshProUGUI errorScreenText;

    public ApiRequests apiRequests;

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
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        nameChangeConfirmButton.onClick.AddListener(OnNameChangeConfirmButtonClick);
        nameChangeCloseButton.onClick.AddListener(OnNameChangeCloseButtonClick);
        changeNameButton.onClick.AddListener(OnChangeNameButtonClick);
        apiRequests.OnUserRegistered += OnUserRegistered;
        apiRequests.OnUserUpdated += OnUserUpdated;
        startButton.onClick.AddListener(OnStartButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);

        // Обновляем текст UserId при старте
        UpdateUserIdText();
    }

    private void OnDestroy()
    {
        confirmButton.onClick.RemoveListener(OnConfirmButtonClick);
        nameChangeConfirmButton.onClick.RemoveListener(OnNameChangeConfirmButtonClick);
        nameChangeCloseButton.onClick.RemoveListener(OnNameChangeCloseButtonClick);
        changeNameButton.onClick.RemoveListener(OnChangeNameButtonClick);
        apiRequests.OnUserRegistered -= OnUserRegistered;
        apiRequests.OnUserUpdated -= OnUserUpdated;
        startButton.onClick.RemoveListener(OnStartButtonClick);
        cancelButton.onClick.RemoveListener(OnCancelButtonClick);
    }

    private void OnConfirmButtonClick()
    {
        string newName = nameInputField.text.Trim();

        if (!IsValidName(newName))
        {
            ShowError("Name must be 1-16 characters long and contain only letters, numbers, and no spaces.");
            return;
        }

        StartCoroutine(apiRequests.RegisterRequest(newName));
    }

    private void OnNameChangeConfirmButtonClick()
    {
        string newName = nameChangeInputField.text.Trim();

        if (!IsValidName(newName))
        {
            ShowError("Name must be 1-16 characters long and contain only letters, numbers, and no spaces.", true);
            return;
        }

        if (PlayerPrefs.HasKey("UserId"))
        {
            string userId = PlayerPrefs.GetString("UserId");
            StartCoroutine(apiRequests.UpdateRequest(userId, newName));
        }
    }



    private void OnNameChangeCloseButtonClick()
    {
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.lobbyScreen);
    }

    private void OnChangeNameButtonClick()
    {
        ClearErrorText();
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.nameChangeScreen);
    }

    private void OnUserRegistered()
    {
        UpdateUserIdText();
        StartCoroutine(apiRequests.GetUserName(PlayerPrefs.GetString("UserId")));
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.lobbyScreen);
    }

    private void OnUserUpdated()
    {
        StartCoroutine(apiRequests.GetUserName(PlayerPrefs.GetString("UserId")));
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.lobbyScreen);
    }


    private void OnStartButtonClick()
    {
        StartCoroutine(apiRequests.JoinQueue());
        StartCoroutine(apiRequests.GetUserProfile());

    }

    private void OnCancelButtonClick()
    {
        StartCoroutine(apiRequests.LeaveQueue());
    }

    public void ShowErrorScreen(string message)
    {
        errorScreenText.text = message;
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.errorScreen);
    }

    public void ShowError(string message, bool isNameChange = false)
    {
        if (isNameChange)
        {
            nameChangeErrorText.gameObject.SetActive(true);
            nameChangeErrorText.text = message;
        }
        else
        {
            errorText.gameObject.SetActive(true);
            errorText.text = message;
        }
    }

    public void ShowBattleScreen()
    {
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.battleScreen);
    }

    public void ShowNameInputScreen()
    {
        ClearErrorText();
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.registrationScreen);
    }

    public void ShowResultScreen(RoundResult roundResult)
    {
        if (roundResult == RoundResult.Win)
        {
            ScreenManager.Instance.ShowScreen(ScreenManager.Instance.victoryScreen);
        }
        else if (roundResult == RoundResult.Lose)
        {
            ScreenManager.Instance.ShowScreen(ScreenManager.Instance.defeatScreen);
        }
        else
        {
            ScreenManager.Instance.ShowScreen(ScreenManager.Instance.drawScreen);
        }
    }

    public void ShowLobbyScreen()
    {
        UpdateUserIdText();
        StartCoroutine(apiRequests.GetUserName(PlayerPrefs.GetString("UserId")));
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.lobbyScreen);
    }

    private void UpdateUserIdText()
    {
        userIdText.gameObject.SetActive(true);
        if (PlayerPrefs.HasKey("UserId"))
        {
            string userId = PlayerPrefs.GetString("UserId");
            userIdText.text = $"UserID: {userId}";
        }
        else
        {
            userIdText.text = "UserID: ";
        }
    }

    public void UpdateUserNameText()
    {
        if (PlayerPrefs.HasKey("UserName"))
        {
            string userName = PlayerPrefs.GetString("UserName");
            userNameText.text = userName;
        }
        else
        {
            userNameText.text = "No Name";
        }
    }

    private bool IsValidName(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length > 16)
        {
            return false;
        }

        string pattern = @"^[a-zA-Zа-яА-Я0-9]+$";
        return Regex.IsMatch(name, pattern);
    }

    private void ClearErrorText()
    {
        errorText.text = "";
        errorText.gameObject.SetActive(false);
        nameChangeErrorText.text = "";
        nameChangeErrorText.gameObject.SetActive(false);
    }



    public void ShowQueueScreen()
    {
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.queueScreen);
        StartCoroutine(QueueProcess());
    }

    private IEnumerator QueueProcess()
    {
        yield return new WaitForSeconds(2);

        while (true)
        {
            yield return new WaitForSeconds(3);

            bool matchCreated = false;
            string opponentId = null;
            yield return StartCoroutine(apiRequests.CheckQueue((result, opponent) =>
            {
                matchCreated = result;
                opponentId = opponent;
            }));

            if (matchCreated && opponentId != null)
            {
                yield return StartCoroutine(apiRequests.GetOpponentData(opponentId));
                yield return StartCoroutine(Countdown());
                ScreenManager.Instance.ShowScreen(ScreenManager.Instance.battleScreen);
                break;
            }
        }
    }

    private IEnumerator Countdown()
    {
        for (int i = 3; i >= 0; i--)
        {
            countdownText.text = $"Match starts in {i}...";
            yield return new WaitForSeconds(1);
        }
    }


    public void UpdateUserProfile(string userName, int matches, int wins)
    {
        userNameText.text = userName;
        userStatsText.text = $"Matches: {matches} / Wins: {wins}";
    }

    public void UpdateQueueUserProfile(string userName, int matches, int wins)
    {
        queueUserNameText.text = userName;
        queueUserStatsText.text = $"Matches: {matches} / Wins: {wins}";
    }

    public void UpdateOpponentProfile(string opponentName, int matches, int wins)
    {
        opponentNameText.text = opponentName;
        opponentStatsText.text = $"Matches: {matches} / Wins: {wins}";
    }
}