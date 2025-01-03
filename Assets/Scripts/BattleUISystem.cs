using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.Mathematics;

public class BattleUISystem : MonoBehaviour
{
    [Header("UI Elements")]
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public GameObject damagePopupPrefab;
    public Toggle playerActionToggle;
    public Toggle enemyActionToggle;

    private Coroutine timerCoroutine;

    private void Awake()
    {
        // Подписываем кнопки на действия
        rockButton.onClick.AddListener(() => FindAnyObjectByType<BattleGameSystem>().PlayerMakeChoice(Choice.Rock));
        paperButton.onClick.AddListener(() => FindAnyObjectByType<BattleGameSystem>().PlayerMakeChoice(Choice.Paper));
        scissorsButton.onClick.AddListener(() => FindAnyObjectByType<BattleGameSystem>().PlayerMakeChoice(Choice.Scissors));
    }

    public void SetActionButtonsInteractable(bool interactable)
    {
        rockButton.interactable = interactable;
        paperButton.interactable = interactable;
        scissorsButton.interactable = interactable;
    }
    public void SetPlayerActionToggle(bool value)
    {
        playerActionToggle.isOn = value;
    }
    public void SetEnemyActionToggle(bool value)
    {
        enemyActionToggle.isOn = value;
    }

    public void UpdateHealthSlider(bool isPlayer, int health)
    {
        Slider slider = isPlayer ? playerHealthSlider : enemyHealthSlider;
        StartCoroutine(SmoothHealthChange(slider, health));
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
        // Определяем слайдер и текущую позицию хэндла
        Slider slider = isPlayer ? playerHealthSlider : enemyHealthSlider;
        RectTransform handleRect = slider.handleRect;
        // Создаем всплывающее окно урона
        GameObject popup = Instantiate(damagePopupPrefab,handleRect.transform.position,quaternion.identity,transform.parent); 

        // Устанавливаем текст урона
        TextMeshProUGUI text = popup.GetComponent<TextMeshProUGUI>();
        text.text = $"-{damage}";

        // Запускаем анимацию всплывающего текста
        StartCoroutine(AnimateDamagePopup(popup));
    }


    private IEnumerator AnimateDamagePopup(GameObject popup)
    {
        RectTransform rect = popup.GetComponent<RectTransform>();
        Vector3 startPosition = rect.localPosition;
        Vector3 endPosition = startPosition + new Vector3(0, -50, 0); // Поднимаем текст на 50 пикселей
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

        timerCoroutine = StartCoroutine(RunRoundTimer(duration, onTimerEnd));
    }

    private IEnumerator RunRoundTimer(float duration, System.Action onTimerEnd)
    {
        float remaining = duration;

        while (remaining > 0)
        {
            timerText.text = remaining.ToString("F1");
            remaining -= Time.deltaTime;
            yield return null;
        }

        timerText.text = "0.0";
        onTimerEnd?.Invoke();
    }

    public void ShowRoundText(int round)
    {
        roundText.text = $"Round {round}";
        StartCoroutine(FadeText(roundText, true));
    }

    public void HideRoundText()
    {
        StartCoroutine(FadeText(roundText, false));
    }

    private IEnumerator FadeText(TextMeshProUGUI text, bool fadeIn)
    {
        float elapsed = 0f;
        float duration = 0.5f;
        Color startColor = text.color;
        Color endColor = startColor;
        endColor.a = fadeIn ? 1f : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        text.color = endColor;
    }
}
