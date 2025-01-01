using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private HealthView _playerHealthView;
    [SerializeField] private float _playerMaxHealth = 100f;

    [Header("Enemy Settings")]
    [SerializeField] private HealthView _enemyHealthView;
    [SerializeField] private float _enemyMaxHealth = 50f;

    private HealthController _playerHealthController;
    private HealthController _enemyHealthController;

    private void Start()
    {
        // Настраиваем игрока
        HealthModel playerModel = new HealthModel(_playerMaxHealth);
        _playerHealthController = new HealthController(playerModel, _playerHealthView);
        _playerHealthController.UpdateHealthBarToFull();

        // Настраиваем врага
        HealthModel enemyModel = new HealthModel(_enemyMaxHealth);
        _enemyHealthController = new HealthController(enemyModel, _enemyHealthView);
        _enemyHealthController.UpdateHealthBarToFull();
    }

    private void Update()
    {
        // Наносим урон игроку по кнопке P
        if (Input.GetKeyDown(KeyCode.P))
        {
            _playerHealthController.ApplyDamage(10f); // Наносим 10 урона игроку
        }

        // Наносим урон врагу по кнопке O
        if (Input.GetKeyDown(KeyCode.O))
        {
            _enemyHealthController.ApplyDamage(10f); // Наносим 10 урона врагу
        }
    }
}
