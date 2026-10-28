using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class InventoryEquipmentPreview : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image _selectedIcon;
    [SerializeField] private UnityEngine.UI.Image _equippedIcon;
    [SerializeField] private TMP_Text _selectedName;
    [SerializeField] private TMP_Text _equippedName;
    [SerializeField] private TMP_Text[] _selectedEnchantments = new TMP_Text[3];
    [SerializeField] private TMP_Text[] _equippedEnchantments = new TMP_Text[3];
    [SerializeField] private TMP_Text[] _selectedStats = new TMP_Text[11];
    [SerializeField] private TMP_Text[] _equippedStats = new TMP_Text[11];

    public static readonly string[] StatLabels =
    {
        "HP", "攻撃力", "スタミナ回復速度", "攻撃速度", "移動速度", "致命攻撃補正",
        "コンボ受付時間", "パリィ受付時間", "ガード時スタミナ消費", "初撃補正", "戦技補正"
    };

    public void ShowComparison(WeaponInstance selected, WeaponInstance equipped)
    {
        ShowIcon(_selectedIcon, selected);
        ShowIcon(_equippedIcon, equipped);
        if (_selectedName != null) _selectedName.text = selected != null ? selected.DisplayName : "なし";
        if (_equippedName != null) _equippedName.text = equipped != null ? equipped.DisplayName : "なし";
        ShowTexts(_selectedEnchantments, GetEnchantmentTexts(selected));
        ShowTexts(_equippedEnchantments, GetEnchantmentTexts(equipped));
        ShowTexts(_selectedStats, GetStatTexts(selected));
        ShowTexts(_equippedStats, GetStatTexts(equipped));
    }

    public static List<string> GetEnchantmentTexts(WeaponInstance weapon)
    {
        var texts = new List<string>();
        for (int i = 0; i < 3; i++)
            texts.Add(EnchantSelectionPanel.FormatEnchantment(weapon?.enchantmentSlots != null && i < weapon.enchantmentSlots.Length
                ? weapon.enchantmentSlots[i] : null));
        return texts;
    }

    public static string[] GetStatTexts(WeaponInstance weapon)
    {
        if (weapon == null)
        {
            var empty = new string[StatLabels.Length];
            for (int i = 0; i < empty.Length; i++) empty[i] = "-";
            return empty;
        }
        return new[]
        {
            weapon.HP.ToString(), weapon.attackPower.ToString(), Format(weapon.staminaRecoveryRate),
            Format(weapon.attackSpeed), Format(weapon.moveSpeed), Format(weapon.criticalDamage),
            Format(weapon.comboBufferTime), Format(weapon.parryBufferTime), Format(weapon.guardStaminaCost),
            Format(weapon.firstAttackBoost), Format(weapon.UniqueAttackBoost)
        };
    }

    private static string Format(float value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private static void ShowIcon(UnityEngine.UI.Image image, WeaponInstance weapon)
    {
        if (image == null) return;
        image.sprite = weapon?.Icon;
        image.enabled = image.sprite != null;
    }

    private static void ShowTexts(TMP_Text[] targets, IReadOnlyList<string> values)
    {
        for (int i = 0; i < targets.Length; i++)
            if (targets[i] != null) targets[i].text = i < values.Count ? values[i] : "-";
    }
}
