using System;
using UnityEngine;

public class HealthModel
{
    public event Action<float> OnHealthChanged; // Событие изменения здоровья

    private float _currentHealth;
    private float _maxHealth;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    public HealthModel(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth / _maxHealth);
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth / _maxHealth);
    }
}
