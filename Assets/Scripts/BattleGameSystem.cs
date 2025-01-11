using UnityEngine;
using System.Collections;

public enum Choice { Empty, Rock, Paper, Scissors }
public enum RoundResult { Empty, Draw, PlayerWin, EnemyWin }

public class BattleGameSystem : MonoBehaviour
{
    public BattleUISystem uiSystem;
    private BattleDetails currentBattle; // Текущий матч

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

    private RoundResult roundResult = RoundResult.Empty;
    private Coroutine enemyActionCoroutine;

    private void Start()
    {
        playerCurrentHealth = playerMaxHealth;
        enemyCurrentHealth = enemyMaxHealth;
        uiSystem.UpdateHealthSlider(true, playerCurrentHealth, playerMaxHealth);
        uiSystem.UpdateHealthSlider(false, enemyCurrentHealth, enemyMaxHealth);
        uiSystem.timerText.text = timerStartValue.ToString();
    }

    public void StartBattle()
    {
        Debug.Log("Starting Battle...");

        currentBattle = new BattleDetails();
        currentRound = 1;

        playerCurrentHealth = playerMaxHealth;
        enemyCurrentHealth = enemyMaxHealth;
        uiSystem.UpdateHealthSlider(true, playerCurrentHealth, playerMaxHealth);
        uiSystem.UpdateHealthSlider(false, enemyCurrentHealth, enemyMaxHealth);
        uiSystem.timerText.text = timerStartValue.ToString();
        uiSystem.playerNameText.text = PlayerPrefs.GetString("PlayerName");
        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        Debug.Log($"Round {currentRound} starts!");

        uiSystem.ResetActionButtons();
        uiSystem.SetActionButtonsInteractable(false);
        roundResult = RoundResult.Empty;
        playerChoice = Choice.Empty;
        enemyChoice = Choice.Empty;
        uiSystem.SetPlayerActionToggle(false);
        uiSystem.SetEnemyActionToggle(false);

        yield return new WaitForSeconds(2);
        uiSystem.ShowAndHideRoundText(currentRound, uiSystem.FadeDuration, uiSystem.VisibleDuration, uiSystem.FadeDuration);
        yield return new WaitForSeconds(uiSystem.FadeDuration * 2 + uiSystem.VisibleDuration);
        yield return new WaitForEndOfFrame();
        uiSystem.SetActionButtonsInteractable(true);

        enemyActionCoroutine = StartCoroutine(EnemyMakeChoice());

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

    private IEnumerator ResolveRound()
    {
        uiSystem.MarkRoundAsOver();
        Debug.Log("Resolving round...");

        RoundDetails roundDetails = new RoundDetails
        {
            RoundNumber = currentRound,
            PlayerChoice = playerChoice,
            EnemyChoice = enemyChoice,
            PlayerHealthChanges = 0,
            EnemyHealthChanges = 0,
            Result = RoundResult.Empty
        };

        if (playerChoice == enemyChoice)
        {
            roundResult = RoundResult.Draw;
            roundDetails.Result = roundResult;

            Debug.Log("It's a draw!");
            roundDetails.Result = roundResult;
            roundDetails.PlayerHealthChanges = damageValue;
            roundDetails.EnemyHealthChanges = damageValue;
            DealDamageToEnemy();
            DealDamageToPlayer();
        }
        else if (
            (playerChoice == Choice.Rock && enemyChoice == Choice.Scissors) ||
            (playerChoice == Choice.Paper && enemyChoice == Choice.Rock) ||
            (playerChoice == Choice.Scissors && enemyChoice == Choice.Paper))
        {
            DealDamageToEnemy();
            roundResult = RoundResult.PlayerWin;
            roundDetails.Result = roundResult;
            roundDetails.EnemyHealthChanges = damageValue;
            Debug.Log("Player wins the round!");
        }
        else
        {
            DealDamageToPlayer();
            roundResult = RoundResult.EnemyWin;
            roundDetails.Result = roundResult;
            roundDetails.PlayerHealthChanges = damageValue;
            Debug.Log("Enemy wins the round!");
        }

        BattleHistory.Instance.AddRound(currentBattle, roundDetails);

        uiSystem.ShowAndHideRoundResultText(roundResult, uiSystem.FadeDuration, uiSystem.VisibleDuration, uiSystem.FadeDuration);
        yield return new WaitForSeconds(uiSystem.FadeDuration * 2 + uiSystem.VisibleDuration);
        yield return new WaitForEndOfFrame();

        currentRound++;
        if (playerCurrentHealth > 0 && enemyCurrentHealth > 0)
        {
            StartCoroutine(StartRound());
        }
        else
        {
            Debug.Log(playerCurrentHealth <= 0 && enemyCurrentHealth <= 0 ? "Enemy wins the game!" : "Player wins the game!");

            if (CanFinishMatch())
            {
                UIManager.Instance.ShowResultScreen(roundResult);
                BattleHistory.Instance.SaveBattleHistory(currentBattle);
            }
        }

    }

    private void DealDamageToPlayer()
    {
        playerCurrentHealth -= damageValue;
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0, playerMaxHealth);
        uiSystem.UpdateHealthSlider(true, playerCurrentHealth, playerMaxHealth);
        uiSystem.ShowDamage(true, damageValue);
    }

    private void DealDamageToEnemy()
    {
        enemyCurrentHealth -= damageValue;
        enemyCurrentHealth = Mathf.Clamp(enemyCurrentHealth, 0, enemyMaxHealth);
        uiSystem.UpdateHealthSlider(false, enemyCurrentHealth, enemyMaxHealth);
        uiSystem.ShowDamage(false, damageValue);
    }

    private void SaveBattleHistory(BattleDetails battle)
    {
        BattleHistory.Instance.AddBattle(battle);
        Debug.Log($"Battle {battle.BattleID} saved with {battle.Rounds.Count} rounds.");
    }

    private void OnRoundTimerEnd()
    {
        if (playerChoice == Choice.Empty)
        {
            uiSystem.SetPlayerActionToggle(false);
        }
        StartCoroutine(ResolveRound());
    }

    private bool CanFinishMatch()
    {
        if (playerCurrentHealth <= 0 && enemyCurrentHealth <= 0) return true;
        if (playerCurrentHealth <= 0 || enemyCurrentHealth <= 0) return true;
        else return false;
    }
}
