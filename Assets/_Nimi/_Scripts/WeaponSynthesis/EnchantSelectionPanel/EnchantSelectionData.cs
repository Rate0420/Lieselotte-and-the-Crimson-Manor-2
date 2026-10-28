using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnchantSelectionData : MonoBehaviour
{
    [SerializeField] private TMP_Text _enchantmentText;

    [SerializeField] private UnityEngine.UI.Image _highlightImage;
    [SerializeField] private UnityEngine.UI.Toggle _selectedToggle;

    public UnityEngine.UI.Selectable Selectable => _selectedToggle;

    public string EnchantmentText { get; private set; }
    public int EnchantmentIndex { get; private set; }

    public event Action<int, bool> OnSelectClicked;

    private bool _isUpdatingToggle;

    private void Awake()
    {
        if (_selectedToggle != null)
        {
            _selectedToggle.transition = UnityEngine.UI.Selectable.Transition.None;
            _selectedToggle.onValueChanged.AddListener(isSelected =>
            {
                if (_isUpdatingToggle) return;

                OnSelectClicked?.Invoke(EnchantmentIndex, isSelected);
            });
        }

        if (_highlightImage != null)
        {
            _highlightImage.raycastTarget = false;
            _highlightImage.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (_highlightImage == null) return;
        _highlightImage.enabled = _selectedToggle != null && _selectedToggle.IsInteractable() &&
            EventSystem.current != null && EventSystem.current.currentSelectedGameObject == _selectedToggle.gameObject;
    }

    private void OnDisable()
    {
        if (_highlightImage != null) _highlightImage.enabled = false;
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
