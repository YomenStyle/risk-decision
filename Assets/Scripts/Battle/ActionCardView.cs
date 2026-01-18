using UnityEngine;
using TMPro;

public class ActionCardView : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text detailText;

    public void SetTitle(string title)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }
    }

    public void SetDetail(string detail)
    {
        if (detailText != null)
        {
            detailText.text = detail;
        }
    }
}
