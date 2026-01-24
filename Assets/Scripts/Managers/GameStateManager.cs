using System;
using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private ChoiceManager choiceManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private LevelManager levelManager;

    [Header("Timing")]
    [SerializeField] private float resultDelaySeconds = 0.5f;
    [SerializeField] private float levelUpDelaySeconds = 0.5f;

    private Coroutine flowRoutine;

    public GameState CurrentState { get; private set; } = GameState.None;

    public event Action<GameState> StateChanged;

    private void Awake()
    {
        if (choiceManager == null)
        {
            choiceManager = FindFirstObjectByType<ChoiceManager>();
        }

        if (battleManager == null)
        {
            battleManager = FindFirstObjectByType<BattleManager>();
        }

        if (levelManager == null)
        {
            levelManager = FindFirstObjectByType<LevelManager>();
        }
    }

    private void OnEnable()
    {
        if (choiceManager != null)
        {
            choiceManager.ChoiceSelected += HandleChoiceSelected;
        }

        if (battleManager != null)
        {
            battleManager.BattleEnded += HandleBattleEnded;
        }
    }

    private void OnDisable()
    {
        if (choiceManager != null)
        {
            choiceManager.ChoiceSelected -= HandleChoiceSelected;
        }

        if (battleManager != null)
        {
            battleManager.BattleEnded -= HandleBattleEnded;
        }
    }

    private void Start()
    {
        SetState(GameState.Choice);
    }

    private void HandleChoiceSelected(int index, ActionData action)
    {
        if (CurrentState != GameState.Choice)
        {
            return;
        }

        SetState(GameState.Battle);
        battleManager.BeginBattle(action);
    }

    private void HandleBattleEnded(BattleResult result)
    {
        if (flowRoutine != null)
        {
            StopCoroutine(flowRoutine);
        }

        flowRoutine = StartCoroutine(AdvanceAfterBattle(result));
    }

    private IEnumerator AdvanceAfterBattle(BattleResult result)
    {
        SetState(GameState.Result);
        if (levelManager != null)
        {
            levelManager.ApplyBattleResult(result);
        }

        if (resultDelaySeconds > 0f)
        {
            yield return new WaitForSeconds(resultDelaySeconds);
        }

        SetState(GameState.LevelUp);

        if (levelUpDelaySeconds > 0f)
        {
            yield return new WaitForSeconds(levelUpDelaySeconds);
        }

        SetState(GameState.Choice);
        choiceManager?.RefreshChoices();
        flowRoutine = null;
    }

    private void SetState(GameState state)
    {
        CurrentState = state;
        StateChanged?.Invoke(state);
        Debug.Log($"State -> {state}");
    }
}

public enum GameState
{
    None,
    Choice,
    Battle,
    Result,
    LevelUp
}
