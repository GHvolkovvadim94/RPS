using System;
using System.Collections.Generic;
using UnityEngine;

public enum BattleResult
{
    Ongoing,
    Draw,
    PlayerWin,
    EnemyWin
}

public class BattleDetails
{
    public Guid BattleID { get; private set; }
    public List<RoundDetails> Rounds { get; private set; }
    public BattleResult Result { get; set; }

    public BattleDetails()
    {
        BattleID = Guid.NewGuid();
        Rounds = new List<RoundDetails>();
        Result = BattleResult.Ongoing;
    }
}
public class RoundDetails
{
    public int RoundNumber { get; set; }
    public Choice PlayerChoice { get; set; }
    public Choice EnemyChoice { get; set; }
    public int PlayerHealthChanges { get; set; }
    public int EnemyHealthChanges { get; set; }
    public RoundResult Result { get; set; }

    public override string ToString()
    {
        return $"Round {RoundNumber}:\n" +
               $"Player choice: {PlayerChoice}, Enemy choice: {EnemyChoice}\n" +
               $"Player health lost: {PlayerHealthChanges}, Enemy health lost: {EnemyHealthChanges}\n" +
               $"Result: {Result}";
    }
}
