using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider; // Слайдер здоровья
    [SerializeField] private GameObject _damageTextPrefab; // Префаб текста урона
    [SerializeField] private float _sliderUpdateSpeed = 0.2f;
    [SerializeField] private float _damageTextMoveDistance = 50f; // Расстояние движения текста (в пикселях)
    [SerializeField] private float _damageTextFadeDuration = 1f; // Время растворения текста

    public void UpdateHealthBar(float healthPercentage)
    {
        StartCoroutine(SmoothUpdateHealthBar(healthPercentage));
    }

    private IEnumerator SmoothUpdateHealthBar(float targetPercentage)
    {
        float startValue = _healthSlider.value;
        float elapsedTime = 0f;

        while (elapsedTime < _sliderUpdateSpeed)
        {
            elapsedTime += Time.deltaTime;
            _healthSlider.value = Mathf.Lerp(startValue, targetPercentage, elapsedTime / _sliderUpdateSpeed);
            yield return null;
        }

        _healthSlider.value = targetPercentage;
    }

    public void ShowDamageText(float damage)
    {
        // Создаем текст урона на Canvas
        GameObject damageTextInstance = Instantiate(
            _damageTextPrefab, 
            _healthSlider.handleRect.position, // Позиция хэндла слайдера
            Quaternion.identity, 
            _healthSlider.transform); // Привязываем к слайдеру

        TextMeshProUGUI damageText = damageTextInstance.GetComponent<TextMeshProUGUI>();
        damageText.text = $"-{damage}";

        // Анимация текста урона
        StartCoroutine(AnimateDamageText(damageTextInstance));
    }

    private IEnumerator AnimateDamageText(GameObject damageTextInstance)
    {
        RectTransform textTransform = damageTextInstance.GetComponent<RectTransform>();
        Vector3 startPosition = textTransform.anchoredPosition; // Используем `anchoredPosition` для Canvas
        Vector3 targetPosition = startPosition + new Vector3(0, _damageTextMoveDistance, 0);

        TextMeshProUGUI textMesh = damageTextInstance.GetComponent<TextMeshProUGUI>();
        Color startColor = textMesh.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        float elapsedTime = 0f;

        while (elapsedTime < _damageTextFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _damageTextFadeDuration;

            // Движение текста вверх
            textTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);

            // Растворение текста
            textMesh.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        Destroy(damageTextInstance); // Удаляем объект после анимации
    }
}
