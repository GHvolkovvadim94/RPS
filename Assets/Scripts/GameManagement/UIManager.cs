using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
    public TextMeshProUGUI userNameText;
    public Button changeNameButton;

    [Header("Global UI Elements")]
    public TextMeshProUGUI userIdText;

    [Header("Loading Screen Elements")]
    public Slider loadingSlider;
    public Button continueButton;

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

    public void ShowErrorScreen(string message)
    {
        errorText.text = message;
        ScreenManager.Instance.ShowScreen(ScreenManager.Instance.errorScreen);
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
}