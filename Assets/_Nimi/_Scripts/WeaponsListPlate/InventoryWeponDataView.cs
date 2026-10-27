using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWeponDataView : MonoBehaviour
{
    [Header("武器情報")]
    [SerializeField] private Image _iconImage;

    [Header("付呪表示")]
    [SerializeField] private TMP_Text[] _enchantmentTexts = new TMP_Text[3];

    [Header("選択表示")]
    [SerializeField] private GameObject _selectedPlane;
    [SerializeField] private TMP_Text _selectedText;
    [SerializeField] private Toggle _selectedToggle;

    public event Action OnSelectClicked;
    private bool _isUpdatingToggle;

    private void Awake()
    {
        ResetSelection();

        if (_selectedToggle != null)
        {
            _selectedToggle.onValueChanged.AddListener(isSelected =>
            {
                if (_isUpdatingToggle) return;

                OnSelectClicked?.Invoke();
            });
        }
    }

    public void SetUpData(Sprite icon, List<string> enchantments)
    {
        SetIcon(icon);
        SetEnchantments(enchantments);
    }

    private void SetIcon(Sprite icon)
    {
        if (_iconImage == null)
        {
            Debug.LogError("IconImage が設定されていません。", this);
            return;
        }

        _iconImage.sprite = icon;
        _iconImage.enabled = icon != null;
    }

    private void SetEnchantments(List<string> enchantments)
    {
        if (_enchantmentTexts == null) return;

        for (int i = 0; i < _enchantmentTexts.Length; i++)
        {
            var text = _enchantmentTexts[i];

            if (text == null)
            {
                Debug.LogWarning($"EnchantmentTexts[{i}] が設定されていません。", this);
                continue;
            }

            text.text = string.Empty;

            if (enchantments != null && i < enchantments.Count)
            {
                text.text = enchantments[i];
            }
        }
    }

    public void ResetSelection()
    {
        SetToggleWithoutEvent(false);

        if (_selectedPlane != null)
        {
            _selectedPlane.SetActive(false);
        }

        if (_selectedText != null)
        {
            _selectedText.text = string.Empty;
        }
    }


    public void Selection(int index)
    {
        SetToggleWithoutEvent(true);

        if (_selectedPlane != null)
        {
            _selectedPlane.SetActive(true);
        }

        if (_selectedText != null)
        {
            _selectedText.text = $"素材 {index}";
        }
    }


    private void SetToggleWithoutEvent(bool value)
    {
        if (_selectedToggle == null) return;

        _isUpdatingToggle = true;
        _selectedToggle.isOn = value;
        _isUpdatingToggle = false;
    }
}