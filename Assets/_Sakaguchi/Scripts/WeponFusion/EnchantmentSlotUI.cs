using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnchantmentSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enchantText;

    public EnchantmentSlot Slot { get; private set; }

    public void Bind(EnchantmentSlot slot)
    {
        Slot = slot;
        enchantText.text = slot.enchantName.ToString() + "+" + slot.enchantLevel;
    }

    public void SetHighlight(bool isCursor, bool isSelected)
    {
        var color = Color.white;

        if (isSelected)
            color = Color.red;
        else if (isCursor)
            // EE0078(ãíì_ÇÃ4Ç¬ÇÃìzÇ∆ìØÇ∂êF)
            color = Color.cyan;

        GetComponent<RawImage>().color = color;
    }
}
