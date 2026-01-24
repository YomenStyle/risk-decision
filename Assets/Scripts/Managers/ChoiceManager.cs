using System;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private ActionData[] choices =
    {
        new ActionData(),
        new ActionData(),
        new ActionData()
    };

    public event Action ChoicesChanged;
    public event Action<int, ActionData> ChoiceSelected;

    private void Start()
    {
        NormalizeChoices();
        ChoicesChanged?.Invoke();
    }

    public int ChoiceCount => choices == null ? 0 : choices.Length;

    public bool IsValidChoiceIndex(int index)
    {
        return choices != null && index >= 0 && index < choices.Length;
    }

    public ActionData GetChoice(int index)
    {
        return IsValidChoiceIndex(index) ? choices[index] : null;
    }

    public void SelectChoice(int index)
    {
        if (!IsValidChoiceIndex(index))
        {
            Debug.LogWarning("Invalid choice index.");
            return;
        }

        ChoiceSelected?.Invoke(index, choices[index]);
    }

    public void RefreshChoices()
    {
        NormalizeChoices();
        ChoicesChanged?.Invoke();
    }

    private void NormalizeChoices()
    {
        if (choices == null || choices.Length == 0)
        {
            choices = new[] { new ActionData() };
        }

        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i] == null)
            {
                choices[i] = new ActionData();
            }

            NormalizeAction(choices[i]);
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
}
