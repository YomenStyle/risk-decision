using System;
using UnityEngine;

[Serializable]
public class Combatant
{
    [SerializeField] private string displayName;
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    [SerializeField] private int failureStack;

    public Combatant(string name)
    {
        displayName = name;
        maxHp = 10;
        currentHp = 10;
        failureStack = 0;
    }

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? "Unknown" : displayName;

    public int MaxHp
    {
        get => maxHp;
        set => maxHp = value;
    }

    public int CurrentHp
    {
        get => currentHp;
        set => currentHp = value;
    }

    public int FailureStack
    {
        get => failureStack;
        set => failureStack = Mathf.Max(0, value);
    }

    public bool IsAlive => currentHp > 0;

    public void ApplyHpDelta(int delta)
    {
        currentHp = Mathf.Clamp(currentHp + delta, 0, maxHp);
    }

    public void TakeDamage(int amount)
    {
        currentHp = Mathf.Max(0, currentHp - amount);
    }
}
