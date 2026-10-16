using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class FrameColor : MonoBehaviour
{
    [SerializeField] private Image frameSprite;
    [SerializeField] private string normalColorCode;
    [SerializeField] private string selectedColorCode;

    private Color nolmalColor;
    private Color selectedColor;

    private void Awake()
    {
        ColorUtility.TryParseHtmlString(normalColorCode, out nolmalColor);
        ColorUtility.TryParseHtmlString(selectedColorCode, out selectedColor);
    }

    public void OnSelect(BaseEventData eventData)
    {
        frameSprite.color = selectedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        frameSprite.color = nolmalColor;
    }
}
