using System;
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Combatants")]
    [SerializeField] private Combatant player = new Combatant("Player");
    [SerializeField] private Combatant enemy = new Combatant("Enemy");

    [Header("Actions")]
    [SerializeField] private ActionData enemyAction = new ActionData();

    [Header("Loop")]
    [SerializeField] private float failureStackBonus = 0.05f;
    [SerializeField] private float turnDelaySeconds = 0.25f;

    private int turnIndex = 1;
    private bool isBattling;
    private Coroutine battleRoutine;

    public event Action<BattleResult> BattleEnded;

    public bool IsBattling => isBattling;

    private void Awake()
    {
        NormalizeCombatant(player);
        NormalizeCombatant(enemy);
        NormalizeAction(enemyAction);
    }

    public void BeginBattle(ActionData playerAction)
    {
        if (isBattling)
        {
            return;
        }

        if (playerAction == null)
        {
            Debug.LogWarning("BattleManager requires a player action.");
            return;
        }

        NormalizeAction(playerAction);
        turnIndex = 1;
        isBattling = true;
        battleRoutine = StartCoroutine(BattleLoop(playerAction));
    }

    private IEnumerator BattleLoop(ActionData playerAction)
    {
        Debug.Log("Battle start (auto)");

        while (player.IsAlive && enemy.IsAlive)
        {
            Debug.Log($"Turn {turnIndex}");

            ExecuteAction(playerAction, player, enemy);
            if (!enemy.IsAlive)
            {
                break;
            }

            ExecuteAction(enemyAction, enemy, player);
            turnIndex++;

            if (turnDelaySeconds > 0f)
            {
                yield return new WaitForSeconds(turnDelaySeconds);
            }
            else
            {
                yield return null;
            }
        }

        EndBattle();
    }

    private void EndBattle()
    {
        if (!isBattling)
        {
            return;
        }

        isBattling = false;
        if (battleRoutine != null)
        {
            StopCoroutine(battleRoutine);
            battleRoutine = null;
        }

        BattleResult result = new BattleResult(player.IsAlive, player, enemy);
        Debug.Log($"Battle end. Winner: {result.WinnerName}");
        BattleEnded?.Invoke(result);
    }

    private void ExecuteAction(ActionData action, Combatant actor, Combatant target)
    {
        float roll = UnityEngine.Random.Range(0f, 1f);
        float adjustedChance = GetAdjustedChance(action, actor);
        Debug.Log($"{actor.DisplayName} action. Roll {roll:0.00} vs {adjustedChance:0.00}");

        if (roll <= adjustedChance)
        {
            ApplyEffect(action.OnSuccessEffect, actor, target);
            Debug.Log($"{target.DisplayName} HP {target.CurrentHp}/{target.MaxHp}");
        }
        else
        {
            ApplyEffect(action.OnFailureEffect, actor, target);
            actor.FailureStack += 1;
            Debug.Log($"{actor.DisplayName} failed. FailureStack {actor.FailureStack}");
        }
    }

    private float GetAdjustedChance(ActionData action, Combatant actor)
    {
        if (action == null || actor == null)
        {
            return 0f;
        }

        float bonus = actor.FailureStack * failureStackBonus;
        return Mathf.Clamp01(action.SuccessChance + bonus);
    }

    private static void ApplyEffect(ActionEffect effect, Combatant actor, Combatant target)
    {
        if (effect == null)
        {
            return;
        }

        effect.Apply(actor, target);
    }

    private static void NormalizeCombatant(Combatant combatant)
    {
        combatant.MaxHp = Mathf.Max(1, combatant.MaxHp);
        combatant.CurrentHp = Mathf.Clamp(combatant.CurrentHp, 1, combatant.MaxHp);
        combatant.FailureStack = Mathf.Max(0, combatant.FailureStack);
    }

    private static void NormalizeAction(ActionData action)
    {
        if (action == null)
        {
            return;
        }

        action.SuccessChance = Mathf.Clamp01(action.SuccessChance);
        if (action.OnSuccessEffect == null)
        {
            action.OnSuccessEffect = new ActionEffect();
        }

        if (action.OnFailureEffect == null)
        {
            action.OnFailureEffect = new ActionEffect();
        }
    }
}

public readonly struct BattleResult
{
    public bool PlayerWon { get; }
    public Combatant Player { get; }
    public Combatant Enemy { get; }

    public string WinnerName => PlayerWon ? Player.DisplayName : Enemy.DisplayName;

    public BattleResult(bool playerWon, Combatant player, Combatant enemy)
    {
        PlayerWon = playerWon;
        Player = player;
        Enemy = enemy;
    }
}
