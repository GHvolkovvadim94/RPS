using System.Collections.Generic;
using UnityEngine;

public class BattleHistory
{
    private static BattleHistory _instance;
    private List<BattleDetails> battleHistory = new List<BattleDetails>();
    private BattleHistory() { }
    public static BattleHistory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BattleHistory();
            }
            return _instance;
        }
    }

    public void AddBattle(BattleDetails battle)
    {
        battleHistory.Add(battle);
    }
    public void AddRound(BattleDetails battle, RoundDetails round)
    {
        battle.Rounds.Add(round);
    }

    public List<BattleDetails> GetHistory()
    {
        return new List<BattleDetails>(battleHistory); // Возвращаем копию списка
    }
    public void SaveBattleHistory(BattleDetails battle)
    {
        Instance.AddBattle(battle);
        Debug.Log($"Battle {battle.BattleID} saved with {battle.Rounds.Count} rounds.");
    }
}
