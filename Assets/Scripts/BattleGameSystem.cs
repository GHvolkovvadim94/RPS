using UnityEngine;
using System.Collections;

public enum Choice { Empty, Rock, Paper, Scissors }

public class BattleGameSystem : MonoBehaviour
{
    public BattleUISystem uiSystem;

    // Игровые переменные
    private int playerHealth = 100;
    private int enemyHealth = 100;
    private Choice playerChoice = Choice.Empty;
    private Choice enemyChoice = Choice.Empty;
    private int currentRound = 1;

    private Coroutine enemyActionCoroutine;

    private void Start()
    {
        StartCoroutine(StartRound());
        uiSystem.UpdateHealthSlider(true,playerHealth);
        uiSystem.UpdateHealthSlider(false,enemyHealth);
    }

    private IEnumerator StartRound()
    {
        Debug.Log($"Round {currentRound} starts!");

        // Отображаем текущий раунд
        uiSystem.ShowRoundText(currentRound);
        yield return new WaitForSeconds(2);
        uiSystem.HideRoundText();

        // Сбрасываем выборы и включаем кнопки для игрока
        playerChoice = Choice.Empty;
        enemyChoice = Choice.Empty;
        uiSystem.SetPlayerActionToggle(false);
        uiSystem.SetEnemyActionToggle(false);
        uiSystem.SetActionButtonsInteractable(true);

        // Запускаем корутину для врага
        enemyActionCoroutine = StartCoroutine(EnemyMakeChoice());

        // Запускаем таймер раунда
        uiSystem.StartRoundTimer(10f, OnRoundTimerEnd);
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
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        enemyChoice = (Choice)Random.Range(1, 4);
        uiSystem.SetEnemyActionToggle(true);
        Debug.Log($"Enemy chose {enemyChoice}");

        CheckRoundResult();
    }

    private void CheckRoundResult()
    {
        if (playerChoice != Choice.Empty && enemyChoice != Choice.Empty)
        {
            ResolveRound();
        }
    }

    private void OnRoundTimerEnd()
    {
        if (playerChoice == Choice.Empty)
        {
            playerChoice = Choice.Empty;
            uiSystem.SetPlayerActionToggle(true);
        }

        if (enemyChoice == Choice.Empty)
        {
            StopCoroutine(enemyActionCoroutine);
            enemyChoice = (Choice)Random.Range(1, 4);
            uiSystem.SetEnemyActionToggle(true);
        }

        ResolveRound();
    }

    private void ResolveRound()
    {
        Debug.Log("Resolving round...");

        if (playerChoice == enemyChoice)
        {
            Debug.Log("It's a draw!");
        }
        else if (
            (playerChoice == Choice.Rock && enemyChoice == Choice.Scissors) ||
            (playerChoice == Choice.Paper && enemyChoice == Choice.Rock) ||
            (playerChoice == Choice.Scissors && enemyChoice == Choice.Paper))
        {
            Debug.Log("Player wins the round!");
            enemyHealth -= 10;
            uiSystem.UpdateHealthSlider(false, enemyHealth);
            uiSystem.ShowDamage(false, 10);
        }
        else
        {
            Debug.Log("Enemy wins the round!");
            playerHealth -= 10;
            uiSystem.UpdateHealthSlider(true, playerHealth);
            uiSystem.ShowDamage(true, 10);
        }

        currentRound++;
        if (playerHealth > 0 && enemyHealth > 0)
        {
            StartCoroutine(StartRound());
        }
        else
        {
            Debug.Log(playerHealth <= 0 ? "Enemy wins the game!" : "Player wins the game!");
        }
    }
}
