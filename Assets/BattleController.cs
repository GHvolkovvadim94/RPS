using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    private int firstRoundNumber = 1;
    private int currentRoundNumber;
    public enum BattleState { Start, Prepare, Moves, Determine, Finish }

    private BattleState state;
    public UnityEvent onCoroutineDead;


    void Start()
    {
        onCoroutineDead = new UnityEvent();
        onCoroutineDead.AddListener(Determine);
        currentRoundNumber = firstRoundNumber;
        state = BattleState.Start;
    }

    public void StartNewBattle()
    {
        Debug.Log("battle is started");
        state = BattleState.Start;
        Prepare(firstRoundNumber);
    }

    public void Prepare(int roundNumber)
    {
        Debug.Log($"preparing of round {currentRoundNumber} is started");
        state = BattleState.Prepare;
    }

    public void Moves()
    {
        Debug.Log("moving is started");
        state = BattleState.Moves;

    }

    public void Determine()
    {
        Debug.Log("determine is started");
        state = BattleState.Determine;
    }

    public void Finish()
    {
        Debug.Log("finish is started");
        state = BattleState.Finish;
    }

    public IEnumerator Countdown(float duration)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (duration == 0)
            {
                onCoroutineDead.Invoke();
                break;
            }
        }
    }
}

