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
    public TextMeshProUGUI playerNameText;


    private Coroutine timerCoroutine;
    private bool roundOver;


    public float FadeDuration { get; private set; } = 1f;
    public float VisibleDuration { get; private set; } = 1f;
    private Button _selectedButton;

    private void Awake()
    {
        rockButton.onClick.AddListener(() => OnPlayerChoice(Choice.Rock, rockButton));
        paperButton.onClick.AddListener(() => OnPlayerChoice(Choice.Paper, paperButton));
        scissorsButton.onClick.AddListener(() => OnPlayerChoice(Choice.Scissors, scissorsButton));

    }
    private void Start()
    {
        roundCanvasGroup.gameObject.SetActive(false);

    }

    private void OnPlayerChoice(Choice choice, Button selectedButton)
    {
        _selectedButton = selectedButton;
        FindAnyObjectByType<BattleGameSystem>().PlayerMakeChoice(choice);
        SetActionButtonsInteractable(false, selectedButton);
    }

    public void SetActionButtonsInteractable(bool interactable, Button selectedButton = null)
    {
        var buttons = new[] { rockButton, paperButton, scissorsButton };

        foreach (var button in buttons)
        {
            if (button == selectedButton)
            {

                button.interactable = false;
                var colors = button.colors;
                colors.disabledColor = colors.normalColor;
                button.colors = colors;
            }
            else
            {
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
            var colors = button.colors;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            button.colors = colors;
        }

        _selectedButton = null;
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

        roundOver = false;
        timerCoroutine = StartCoroutine(RunRoundTimer(duration, onTimerEnd));
    }
    public void StopRoundTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        timerText.text = "0.0";
    }

    private IEnumerator RunRoundTimer(float duration, System.Action onTimerEnd)
    {
        float remaining = duration;

        while (remaining > 0)
        {
            if (roundOver)
            {
                yield break;
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
        roundOver = true;
        StopRoundTimer();
    }

    public void ShowAndHideRoundText(int round, float fadeInDuration, float visibleDuration, float fadeOutDuration)
    {
        roundText.text = $"Round {round}";
        roundCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeInAndOutCanvasGroup(roundCanvasGroup, fadeInDuration, visibleDuration, fadeOutDuration));
    }
    public void ShowAndHideRoundResultText(RoundResult roundResult, float fadeInDuration, float visibleDuration, float fadeOutDuration)
    {
        switch (roundResult)
        {
            case RoundResult.Draw:
                roundText.text = $"It's a draw";
                break;
            case RoundResult.PlayerWin:
                roundText.text = $"You won";
                break;
            case RoundResult.EnemyWin:
                roundText.text = $"Enemy won";
                break;


        }
        roundCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeInAndOutCanvasGroup(roundCanvasGroup, fadeInDuration, visibleDuration, fadeOutDuration));
    }

    private IEnumerator FadeInAndOutCanvasGroup(CanvasGroup canvasGroup, float fadeInDuration, float visibleDuration, float fadeOutDuration)
    {
        // Fade in
        float elapsed = 0f;
        float startAlpha = 0f;
        float targetAlpha = 1f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        yield return new WaitForSeconds(visibleDuration);

        // Fade out
        elapsed = 0f;
        startAlpha = 1f;
        targetAlpha = 0f;
        yield return new WaitForEndOfFrame();

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        yield return new WaitForEndOfFrame();
        canvasGroup.gameObject.SetActive(false);
    }
}
