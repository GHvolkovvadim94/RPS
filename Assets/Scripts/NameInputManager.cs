using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInputManager : MonoBehaviour
{
    // Ссылки на UI элементы
    public TMP_InputField nameInputField;
    public Button submitButton;
    public Button closeButton;
    public TextMeshProUGUI errorText;

    public delegate void OnNameChanged(string newName);
    public static event OnNameChanged NameChanged;


    private const string PlayerNameKey = "PlayerName";
    private const int MaxNameLength = 16;

    private void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
        closeButton.onClick.AddListener(() => Close());
        nameInputField.characterLimit = MaxNameLength;
        errorText.gameObject.SetActive(false);

    }

    private void OnSubmit()
    {
        string newName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(newName))
        {
            ShowError("Input field cannot be empty.");
            return;
        }

        PlayerPrefs.SetString(PlayerNameKey, newName);
        PlayerPrefs.Save();

        NameChanged?.Invoke(newName);

        UIManager.Instance.nameInputScreen.SetActive(false);
        ResetUI();
    }

    private void Close()
    {
        UIManager.Instance.nameInputScreen.SetActive(false);
        ResetUI();
    }
    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }

    private void ResetUI()
    {
        nameInputField.text = null;
        errorText.text = null;
    }
}
