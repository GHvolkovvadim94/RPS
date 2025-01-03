using UnityEngine;
using System.Collections;

public enum Choice { Empty, Rock, Paper, Scissors }

public class BattleGameSystem : MonoBehaviour
{
    public BattleUISystem uiSystem;

    // Игровые переменные
    private int playerMaxHealth = 100;
    private int playerCurrentHealth;
    private int enemyMaxHealth = 100;
    private int damageValue = 20;
    private int enemyCurrentHealth;
    private Choice playerChoice = Choice.Empty;
    private Choice enemyChoice = Choice.Empty;
    private int currentRound = 1;
    private float timerStartValue = 5f;

    private Coroutine enemyActionCoroutine;

    private void Start()
    {
        playerCurrentHealth = playerMaxHealth;
        enemyCurrentHealth = enemyMaxHealth;
        uiSystem.UpdateHealthSlider(true, playerCurrentHealth, playerMaxHealth);
        uiSystem.UpdateHealthSlider(false, enemyCurrentHealth, enemyMaxHealth);
        uiSystem.timerText.text = timerStartValue.ToString();
        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        Debug.Log($"Round {currentRound} starts!");

        uiSystem.ResetActionButtons();
        uiSystem.SetActionButtonsInteractable(false);
        
        playerChoice = Choice.Empty;
        enemyChoice = Choice.Empty;
        uiSystem.SetPlayerActionToggle(false);
        uiSystem.SetEnemyActionToggle(false);


        uiSystem.ShowRoundText(currentRound, uiSystem.FadeDuration);
        yield return new WaitForSeconds(2);
        uiSystem.HideRoundText(uiSystem.FadeDuration);
        yield return new WaitForSeconds(uiSystem.FadeDuration);
        uiSystem.SetActionButtonsInteractable(true);


        // Запускаем корутину для врага
        enemyActionCoroutine = StartCoroutine(EnemyMakeChoice());

        // Запускаем таймер раунда
        uiSystem.StartRoundTimer(timerStartValue, OnRoundTimerEnd);
    }

    public void PlayerMakeChoice(Choice choice)
    {
        if (playerChoice == Choice.Empty)
        {
            playerChoice = choice;
            uiSystem.SetPlayerActionToggle(true);
            uiSystem.SetActionButtonsInteractable(false);
            Debug.Log($"Player chose {choice}");

            CheckRoundResult();
        }
    }

    private IEnumerator EnemyMakeChoice()
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        enemyChoice = (Choice)Random.Range(1, 4);
        uiSystem.SetEnemyActionToggle(true);
        Debug.Log($"Enemy chose {enemyChoice}");

        CheckRoundResult();
    }

    private void CheckRoundResult()
    {
        if (playerChoice != Choice.Empty && enemyChoice != Choice.Empty)
        {
            StartCoroutine(ResolveRound());
        }
    }

    private void OnRoundTimerEnd()
    {
        if (playerChoice == Choice.Empty)
        {
            uiSystem.SetPlayerActionToggle(false);
        }
        StartCoroutine(ResolveRound());
    }

    private IEnumerator ResolveRound()
    {
        uiSystem.MarkRoundAsOver();
        Debug.Log("Resolving round...");

        if (playerChoice == enemyChoice)
        {
            Debug.Log("It's a draw!");
            DealDamageToEnemy();
            DealDamageToPlayer();

        }
        else if (
            (playerChoice == Choice.Rock && enemyChoice == Choice.Scissors) ||
            (playerChoice == Choice.Paper && enemyChoice == Choice.Rock) ||
            (playerChoice == Choice.Scissors && enemyChoice == Choice.Paper))
        {
            DealDamageToEnemy();
            Debug.Log("Player wins the round!");

        }
        else
        {
            DealDamageToPlayer();
            Debug.Log("Enemy wins the round!");

        }
        yield return new WaitForSeconds(2);

        currentRound++;
        if (playerCurrentHealth > 0 && enemyCurrentHealth > 0)
        {
            StartCoroutine(StartRound());
        }
        else
        {
            Debug.Log(playerCurrentHealth <= 0 ? "Enemy wins the game!" : "Player wins the game!");
        }

    }
    private void DealDamageToPlayer()
    {
        playerCurrentHealth -= damageValue;
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0, playerMaxHealth); // Ограничиваем здоровье от 0 до максимума
        uiSystem.UpdateHealthSlider(true, playerCurrentHealth, playerMaxHealth); // Указываем true для обновления здоровья игрока
        uiSystem.ShowDamage(true, damageValue); // true для отображения урона для игрока
    }

    private void DealDamageToEnemy()
    {
        enemyCurrentHealth -= damageValue;
        enemyCurrentHealth = Mathf.Clamp(enemyCurrentHealth, 0, enemyMaxHealth); // Ограничиваем здоровье от 0 до максимума
        uiSystem.UpdateHealthSlider(false, enemyCurrentHealth, enemyMaxHealth); // false для обновления здоровья врага
        uiSystem.ShowDamage(false, damageValue); // false для отображения урона для врага
    }
}
