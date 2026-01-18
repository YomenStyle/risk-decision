using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleActionUI : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private RectTransform container;
    [SerializeField] private Button actionButtonPrefab;
    [SerializeField] private string[] actionLabels = { "안전", "중간", "도박" };
    [SerializeField] private bool showSuccessChance = true;
    [SerializeField] private string successChanceFormat = "성공 {0:0}%";

    private void Awake()
    {
        if (battleManager == null)
        {
            battleManager = FindObjectOfType<BattleManager>();
        }

        EnsureEventSystem();

        if (container == null)
        {
            Debug.LogWarning("BattleActionUI requires a container reference.");
        }
    }

    private void Start()
    {
        if (battleManager == null || container == null || actionButtonPrefab == null)
        {
            Debug.LogWarning("BattleActionUI missing references.");
            return;
        }

        BuildButtons();
    }

    // UI is provided by the scene; assign container in the inspector.

    private void BuildButtons()
    {
        ClearChildren(container);

        int count = battleManager.PlayerActionCount;
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
                ActionData action = battleManager.GetPlayerAction(index);
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
        button.onClick.AddListener(() => battleManager.TryExecutePlayerAction(capturedIndex));
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
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemGo = new GameObject("EventSystem");
        eventSystemGo.AddComponent<EventSystem>();
        eventSystemGo.AddComponent<StandaloneInputModule>();
    }
}
