public class HealthController
{
    private HealthModel _healthModel;
    private HealthView _healthView;

    public HealthController(HealthModel healthModel, HealthView healthView)
    {
        _healthModel = healthModel;
        _healthView = healthView;

        _healthModel.OnHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(float healthPercentage)
    {
        _healthView.UpdateHealthBar(healthPercentage);
    }

    public void ApplyDamage(float damage)
    {
        _healthModel.TakeDamage(damage);
        _healthView.ShowDamageText(damage);
    }

    public void ApplyHealing(float healing)
    {
        _healthModel.Heal(healing);
    }

    // Новый метод для обновления полоски здоровья на 100%
    public void UpdateHealthBarToFull()
    {
        _healthView.UpdateHealthBar(1f);
    }
}
