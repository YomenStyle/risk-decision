using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int currentXp;
    [SerializeField] private int xpToNextLevel = 5;
    [SerializeField] private int winXp = 3;
    [SerializeField] private int lossXp = 1;

    public event Action<int> LevelUp;

    public int Level => level;
    public int CurrentXp => currentXp;
    public int XpToNextLevel => xpToNextLevel;

    public void ApplyBattleResult(BattleResult result)
    {
        int gainedXp = result.PlayerWon ? winXp : lossXp;
        GainXp(gainedXp);
    }

    private void GainXp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentXp += amount;
        while (currentXp >= xpToNextLevel)
        {
            currentXp -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.Max(1, Mathf.RoundToInt(xpToNextLevel * 1.2f));
            LevelUp?.Invoke(level);
            Debug.Log($"Level up -> {level}");
        }
    }
}
