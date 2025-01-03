using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BattleUISystem : MonoBehaviour
{
    [Header("UI Elements")]
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
     public TextMeshProUGUI playerHealthText; 
    public TextMeshProUGUI enemyHealthText;
    [SerializeField] private CanvasGroup roundCanvasGroup;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public GameObject damagePopupPrefab;
    public Toggle playerActionToggle;
    public Toggle enemyActionToggle;

    private Coroutine timerCoroutine;
    private bool roundOver;


    public float FadeDuration { get; private set; } = 1f;
    private Button _selectedButton;

    private void Awake()
    {
        // Подписываем кнопки на действия
        rockButton.onClick.AddListener(() => OnPlayerChoice(Choice.Rock, rockButton));
        paperButton.onClick.AddListener(() => OnPlayerChoice(Choice.Paper, paperButton));
        scissorsButton.onClick.AddListener(() => OnPlayerChoice(Choice.Scissors, scissorsButton));
    }

    private void OnPlayerChoice(Choice choice, Button selectedButton)
    {
        _selectedButton = selectedButton;
        FindAnyObjectByType<BattleGameSystem>().PlayerMakeChoice(choice);

        // Отключаем все кнопки, кроме выбранной
        SetActionButtonsInteractable(false, selectedButton);
    }

    public void SetActionButtonsInteractable(bool interactable, Button selectedButton = null)
    {
        var buttons = new[] { rockButton, paperButton, scissorsButton };

        foreach (var button in buttons)
        {
            if (button == selectedButton)
            {
                // Делаем кнопку неактивной, но убираем затемнение
                button.interactable = false;
                var colors = button.colors;
                colors.disabledColor = colors.normalColor; // Убираем затемнение
                button.colors = colors;
            }
            else
            {
                // Делаем остальные кнопки неактивными с затемнением
                button.interactable = interactable;
            }
        }
    }

    public void ResetActionButtons()
    {
        var buttons = new[] { rockButton, paperButton, scissorsButton };

        foreach (var button in buttons)
        {
            button.interactable = true;

            // Восстанавливаем затемнение
            var colors = button.colors;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            button.colors = colors;
        }

        _selectedButton = null; // Сбрасываем выбранную кнопку
    }

    public void SetPlayerActionToggle(bool value)
    {
        playerActionToggle.isOn = value;
    }

    public void SetEnemyActionToggle(bool value)
    {
        enemyActionToggle.isOn = value;
    }

    public void UpdateHealthSlider(bool isPlayer, int currentHealth, int maxHealth)
    {
        Slider slider = isPlayer ? playerHealthSlider : enemyHealthSlider;
        StartCoroutine(SmoothHealthChange(slider, currentHealth));
        TextMeshProUGUI healthText = isPlayer ? playerHealthText : enemyHealthText;
        healthText.text = $"{currentHealth}/{maxHealth}";
    }

    private IEnumerator SmoothHealthChange(Slider slider, int targetHealth)
    {
        float currentHealth = slider.value;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(currentHealth, targetHealth, elapsed / duration);
            yield return null;
        }

        slider.value = targetHealth;
    }

    public void ShowDamage(bool isPlayer, int damage)
    {
        Slider slider = isPlayer ? playerHealthSlider : enemyHealthSlider;
        RectTransform handleRect = slider.handleRect;
        Vector3 handlePosition = handleRect.position;

        GameObject popup = Instantiate(damagePopupPrefab, handleRect.transform.parent);
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.position = handlePosition;

        TextMeshProUGUI text = popup.GetComponent<TextMeshProUGUI>();
        text.text = $"-{damage}";

        StartCoroutine(AnimateDamagePopup(popup));
    }

    private IEnumerator AnimateDamagePopup(GameObject popup)
    {
        RectTransform rect = popup.GetComponent<RectTransform>();
        Vector3 startPosition = rect.localPosition;
        Vector3 endPosition = startPosition + new Vector3(0, -50, 0);
        Color startColor = popup.GetComponent<TextMeshProUGUI>().color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rect.localPosition = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            popup.GetComponent<TextMeshProUGUI>().color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        Destroy(popup);
    }


    public void StartRoundTimer(float duration, System.Action onTimerEnd)
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        roundOver = false; // Сбрасываем флаг перед началом нового раунда
        timerCoroutine = StartCoroutine(RunRoundTimer(duration, onTimerEnd));
    }
    public void StopRoundTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        timerText.text = "0.0"; // Можно установить финальное значение таймера, если нужно
    }

    private IEnumerator RunRoundTimer(float duration, System.Action onTimerEnd)
    {
        float remaining = duration;

        while (remaining > 0)
        {
            if (roundOver) // Проверяем, завершился ли раунд
            {
                yield break; // Прерываем выполнение корутины
            }

            timerText.text = remaining.ToString("F1");
            remaining -= Time.deltaTime;
            yield return null;
        }

        timerText.text = "0.0";
        onTimerEnd?.Invoke();
    }

    public void MarkRoundAsOver()
    {
        roundOver = true; // Устанавливаем флаг завершения раунда
        StopRoundTimer(); // Останавливаем таймер
    }

    public void ShowRoundText(int round, float duration)
    {
        roundText.text = $"Round {round}";
        StartCoroutine(FadeCanvasGroup(roundCanvasGroup, duration, true));
    }

    public void HideRoundText(float duration)
    {
        StartCoroutine(FadeCanvasGroup(roundCanvasGroup, duration, false));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float duration, bool fadeIn)
    {
        float elapsed = 0f;

        float startAlpha = canvasGroup.alpha;
        float targetAlpha = fadeIn ? 1f : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (!fadeIn)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
