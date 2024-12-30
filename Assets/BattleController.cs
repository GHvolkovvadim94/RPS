using UnityEngine;
public class BattleController : MonoBehaviour
{
    private int firstRoundNumber = 1;
    public int currentRoundNumber;
    public enum BattleState { Start, Prepare, Moves, Determine, Finish }

    private BattleState state;

    void Start()
    {
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
        Debug.Log($"preparing of round {roundNumber} is started");
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
}

