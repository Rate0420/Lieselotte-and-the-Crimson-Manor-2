using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextCollar : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;            // 変更したいテキスト
    [SerializeField] private string normalColorCode = "#9AA60D";     // 変更前の色
    [SerializeField] private string selectedColorCode = "#EE0078";   // 変更後の色

    private Color normalColor;
    private Color selectedColor;

    private void Awake()
    {
        ColorUtility.TryParseHtmlString(normalColorCode, out normalColor);
        ColorUtility.TryParseHtmlString(selectedColorCode, out selectedColor);

        buttonText.color = normalColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        buttonText.color = selectedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonText.color = normalColor;
    }
}