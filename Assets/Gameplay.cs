using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    public float timerValue = 5;
    public float timeToEnemyTurn = 2;
    public Choice playerCloice;
    public Choice EnemyChoice;
    public bool isPlayerReady;
    public Toggle PlayerReadyToggle;
    public bool isEnemyReady;
    public Toggle EnemyReadyToggle;

    public TextMeshProUGUI TimerGUI;

    void Start()
    {
        TimerGUI.text = timerValue.ToString();
        StartCoroutine(Countdown(timerValue, TimerGUI));
        StartCoroutine(Countdown(timeToEnemyTurn));
    }

    void Update()
    {
    }

    private IEnumerator Countdown(float duration, TextMeshProUGUI textObject)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            duration--;
            textObject.text = duration.ToString();
            if (duration == 0)
            {
                PlayerIsReady();
                playerCloice = Choice.Paper;
                Debug.Log("Все, пизда!");
                break;
            }
        }
    }
    private IEnumerator Countdown(float duration)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            duration--;
            if (duration == 0)
            {
                EnemyIsReady();
                EnemyChoice = Choice.Rock;
                break;
            }
        }
    }
    private void EnemyIsReady()
    {
        isEnemyReady = true;
        EnemyReadyToggle.isOn = true;
    }
    private void PlayerIsReady()
    {
        isPlayerReady = true;
        PlayerReadyToggle.isOn = true;
    }
}
