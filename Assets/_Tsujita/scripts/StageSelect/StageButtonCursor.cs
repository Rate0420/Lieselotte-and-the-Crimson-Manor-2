using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StageButtonCursor : MonoBehaviour,
    ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject infoPanel;  // ステージごとのパネル

    [SerializeField] private Image frameSprite;
    [SerializeField] private string normalColorCode = "FFFFFF";
    [SerializeField] private string selectedColorCode = "FF0000";

    [SerializeField] private int stageNo;
    [SerializeField] private TextMeshProUGUI higeScoreText;

    private int higeScore;
    
    private Color nolmalColor;
    private Color selectedColor;

    private void Awake()
    {
        ColorUtility.TryParseHtmlString(normalColorCode, out nolmalColor);
        ColorUtility.TryParseHtmlString(selectedColorCode, out selectedColor);
        higeScore = PlayerPrefs.GetInt("HigeScore" + stageNo);
        higeScoreText.text = "ハイスコア ： " + higeScore;
    }

    private void Start()
    {
        infoPanel.SetActive(false);
    }

    /// <summary>
    /// 選択中
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        infoPanel.SetActive(true);
        frameSprite.color = Color.red;
    }

    /// <summary>
    /// 非選択
    /// </summary>
    public void OnDeselect(BaseEventData eventData)
    {
        infoPanel.SetActive(false);
        frameSprite.color = nolmalColor;
    }
}
