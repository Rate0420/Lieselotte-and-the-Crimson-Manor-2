using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnchantSelectionData : MonoBehaviour
{
    [SerializeField] private TMP_Text _enchantmentText;
    [SerializeField] private Toggle _selectedToggle;

    public string EnchantmentText { get; private set; }
    public int EnchantmentIndex { get; private set; }

    public event Action<int, bool> OnSelectClicked;

    private bool _isUpdatingToggle;

    private void Awake()
    {
        if (_selectedToggle != null)
        {
            _selectedToggle.onValueChanged.AddListener(isSelected =>
            {
                if (_isUpdatingToggle) return;

                OnSelectClicked?.Invoke(EnchantmentIndex, isSelected);
            });
        }
    }

    public void SetUp(int index, string enchantment)
    {
        EnchantmentIndex = index;
        EnchantmentText = enchantment;

        if (_enchantmentText != null)
        {
            _enchantmentText.text = enchantment;
        }
    }

    public void Selected(bool isSelected)
    {
        if (_selectedToggle == null) return;

        _isUpdatingToggle = true;
        _selectedToggle.isOn = isSelected;
        _isUpdatingToggle = false;
    }
}