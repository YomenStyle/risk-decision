using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Combatants")]
    [SerializeField] private Combatant player = new Combatant("Player");
    [SerializeField] private Combatant enemy = new Combatant("Enemy");

    [Header("Actions")]
    [SerializeField] private ActionData[] playerActions = new ActionData[]
    {
        new ActionData(),
        new ActionData(),
        new ActionData()
    };
    [SerializeField] private ActionData enemyAction = new ActionData();

    [Header("Loop")]
    [SerializeField] private float failureStackBonus = 0.05f;

    private int turnIndex = 1;

    private void Start()
    {
        NormalizeCombatant(player);
        NormalizeCombatant(enemy);
        NormalizePlayerActions();
        NormalizeAction(enemyAction);
        Debug.Log("Battle start. Choose an action to advance.");
    }

    public void TryExecutePlayerAction(int actionIndex)
    {
        if (!player.IsAlive || !enemy.IsAlive)
        {
            LogWinner();
            return;
        }

        if (!IsValidPlayerActionIndex(actionIndex))
        {
            Debug.Log("Invalid action index.");
            return;
        }

        Debug.Log($"Turn {turnIndex}");
        ExecuteAction(playerActions[actionIndex], player, enemy);

        if (!enemy.IsAlive)
        {
            LogWinner();
            return;
        }

        ExecuteAction(enemyAction, enemy, player);
        turnIndex++;

        if (!player.IsAlive)
        {
            LogWinner();
        }
    }

    public int PlayerActionCount => playerActions == null ? 0 : playerActions.Length;

    public bool IsValidPlayerActionIndex(int index)
    {
        return playerActions != null && index >= 0 && index < playerActions.Length;
    }

    public ActionData GetPlayerAction(int index)
    {
        return IsValidPlayerActionIndex(index) ? playerActions[index] : null;
    }

    private void ExecuteAction(ActionData action, Combatant actor, Combatant target)
    {
        float roll = Random.Range(0f, 1f);
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

    private void NormalizePlayerActions()
    {
        if (playerActions == null || playerActions.Length == 0)
        {
            playerActions = new ActionData[] { new ActionData() };
        }

        for (int i = 0; i < playerActions.Length; i++)
        {
            if (playerActions[i] == null)
            {
                playerActions[i] = new ActionData();
            }

            NormalizeAction(playerActions[i]);
        }
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

    private void LogWinner()
    {
        string winner = player.IsAlive ? player.DisplayName : enemy.DisplayName;
        Debug.Log($"Battle end. Winner: {winner}");
    }
}
