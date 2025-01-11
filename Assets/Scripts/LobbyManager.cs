using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public Button changeNameButton;
    public Button playButton;

    private void Start()
    {
        // Загружаем имя игрока из PlayerPrefs или устанавливаем дефолтное
        string playerName = PlayerPrefs.GetString("PlayerName", "NewPlayer01");
        playerNameText.text = playerName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerNameText.transform.parent.GetComponent<RectTransform>());


        NameInputManager.NameChanged += OnNameChanged;

        playButton.onClick.AddListener(OnPlayButtonPressed);
        changeNameButton.onClick.AddListener(OnChangeNameButtonPressed);

    }

    private void LoadPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string playerName = PlayerPrefs.GetString("PlayerName");
            playerNameText.text = playerName;
        }
        else
        {
            playerNameText.text = "NONAME";
        }
    }

    private void OnPlayButtonPressed()
    {
        UIManager.Instance.ShowBattleScreen();
    }
    private void OnChangeNameButtonPressed()
    {
        UIManager.Instance.ShowNameInputScreen();
    }
    private void OnNameChanged(string newName)
    {
        playerNameText.text = newName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerNameText.transform.parent.GetComponent<RectTransform>());
    }
    private void OnDestroy()
    {
        NameInputManager.NameChanged -= OnNameChanged;
    }
}
