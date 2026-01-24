using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleActionUI : MonoBehaviour
{
    [SerializeField] private ChoiceManager choiceManager;
    [SerializeField] private RectTransform container;
    [SerializeField] private Button actionButtonPrefab;
    [SerializeField] private string[] actionLabels = { "안전", "중간", "도박" };
    [SerializeField] private bool showSuccessChance = true;
    [SerializeField] private string successChanceFormat = "성공 {0:0}%";

    private void Awake()
    {
        if (choiceManager == null)
        {
            choiceManager = FindFirstObjectByType<ChoiceManager>();
        }

        EnsureEventSystem();

        if (container == null)
        {
            Debug.LogWarning("BattleActionUI requires a container reference.");
        }
    }

    private void OnEnable()
    {
        if (choiceManager != null)
        {
            choiceManager.ChoicesChanged += BuildButtons;
        }
    }

    private void OnDisable()
    {
        if (choiceManager != null)
        {
            choiceManager.ChoicesChanged -= BuildButtons;
        }
    }

    private void Start()
    {
        if (choiceManager == null || container == null || actionButtonPrefab == null)
        {
            Debug.LogWarning("BattleActionUI missing references.");
            return;
        }

        BuildButtons();
    }

    private void BuildButtons()
    {
        if (choiceManager == null || container == null || actionButtonPrefab == null)
        {
            return;
        }

        ClearChildren(container);

        int count = choiceManager.ChoiceCount;
        for (int i = 0; i < count; i++)
        {
            string label = GetLabel(i);
            CreateButton(container, label, i);
        }
    }

    private string GetLabel(int index)
    {
        if (actionLabels != null && index < actionLabels.Length && !string.IsNullOrWhiteSpace(actionLabels[index]))
        {
            return actionLabels[index];
        }

        return $"Action {index + 1}";
    }

    private void CreateButton(RectTransform parent, string label, int index)
    {
        Button button = Instantiate(actionButtonPrefab, parent);
        button.name = $"ActionButton{index + 1}";

        ActionCardView cardView = button.GetComponent<ActionCardView>();
        if (cardView != null)
        {
            cardView.SetTitle(label);
            if (showSuccessChance)
            {
                ActionData action = choiceManager.GetChoice(index);
                float chancePercent = action != null ? action.SuccessChance * 100f : 0f;
                cardView.SetDetail(string.Format(successChanceFormat, chancePercent));
            }
        }
        else
        {
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = label;
            }
            else
            {
                Debug.LogWarning($"Action button prefab missing TMP_Text component: {button.name}");
            }
        }

        if (cardView == null && button.GetComponentInChildren<TMP_Text>() == null)
        {
            Debug.LogWarning($"Action button prefab missing UI label: {button.name}");
        }

        int capturedIndex = index;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => choiceManager.SelectChoice(capturedIndex));
    }

    private static void ClearChildren(RectTransform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemGo = new GameObject("EventSystem");
        eventSystemGo.AddComponent<EventSystem>();
        eventSystemGo.AddComponent<StandaloneInputModule>();
    }
}
