using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsPreviewPlate : MonoBehaviour
{
    [Header("プレビュー01")]
    [SerializeField] private UnityEngine.UI.Image _icon01;
    [SerializeField] private TMP_Text[] _enchantmentTexts01 = new TMP_Text[3];

    [Header("プレビュー02")]
    [SerializeField] private UnityEngine.UI.Image _icon02;
    [SerializeField] private TMP_Text[] _enchantmentTexts02 = new TMP_Text[3];

    [Header("合成結果プレビュー03")]
    [SerializeField] private UnityEngine.UI.Image _icon03;
    [SerializeField] private TMP_Text[] _enchantmentTexts03 = new TMP_Text[3];

    /// <summary>
    /// 選択した武器をプレビュー01または02に表示する
    /// </summary>
    public void SetWeaponPreview(int previewIndex, Sprite icon, List<string> enchantments)
    {
        UnityEngine.UI.Image targetIcon;
        TMP_Text[] targetTexts;

        if (previewIndex == 0)
        {
            targetIcon = _icon01;
            targetTexts = _enchantmentTexts01;
        }
        else if (previewIndex == 1)
        {
            targetIcon = _icon02;
            targetTexts = _enchantmentTexts02;
        }
        else
        {
            return;
        }

        SetIcon(targetIcon, icon);
        SetEnchantmentTexts(targetTexts, enchantments);
    }

    /// <summary>
    /// 合成結果の武器アイコンを設定する
    /// </summary>
    public void SetResultIcon(Sprite icon)
    {
        SetIcon(_icon03, icon);
    }

    /// <summary>
    /// 合成結果の付呪効果を設定する
    /// </summary>
    public void SetResultEnchantments(List<string> enchantments)
    {
        SetEnchantmentTexts(_enchantmentTexts03, enchantments);
    }

    /// <summary>
    /// プレビューをすべて初期化する
    /// </summary>
    public void ClearPreview()
    {
        SetIcon(_icon01, null);
        SetIcon(_icon02, null);
        SetIcon(_icon03, null);

        SetEnchantmentTexts(_enchantmentTexts01, null);
        SetEnchantmentTexts(_enchantmentTexts02, null);
        SetEnchantmentTexts(_enchantmentTexts03, null);
    }

    private void SetIcon(UnityEngine.UI.Image iconImage, Sprite icon)
    {
        if (iconImage == null) return;

        iconImage.sprite = icon;
        iconImage.enabled = icon != null;
    }

    private void SetEnchantmentTexts(TMP_Text[] texts, List<string> enchantments)
    {
        if (texts == null) return;

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null) continue;

            texts[i].text = enchantments != null && i < enchantments.Count
                ? enchantments[i]
                : "なし";
        }
    }
}
