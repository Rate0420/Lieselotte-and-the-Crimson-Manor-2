using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryWeponDataView : MonoBehaviour
{
    [Header("武器情報")]
    [SerializeField] private UnityEngine.UI.Image _iconImage;

    [Header("付呪表示")]
    [SerializeField] private TMP_Text[] _enchantmentTexts = new TMP_Text[3];

    [Header("選択表示")]
    [SerializeField] private GameObject _selectedPlane;
    [SerializeField] private UnityEngine.UI.Image _highlightImage;
    [SerializeField] private TMP_Text _selectedText;
    [SerializeField] private UnityEngine.UI.Toggle _selectedToggle;

    public UnityEngine.UI.Selectable Selectable => _selectedToggle;

    public event Action<InventoryWeponDataView> OnSelectClicked;
    private bool _isUpdatingToggle;

    private void Awake()
    {
        ResetSelection();

        if (_selectedToggle != null)
        {
            _selectedToggle.transition = UnityEngine.UI.Selectable.Transition.None;
            _selectedToggle.onValueChanged.AddListener(isSelected =>
            {
                if (_isUpdatingToggle) return;

                OnSelectClicked?.Invoke(this);
            });
        }

        if (_highlightImage != null) _highlightImage.raycastTarget = false;
    }

    private void LateUpdate()
    {
        UpdateHighlight();
    }

    private void OnDisable()
    {
        if (_highlightImage != null) _highlightImage.enabled = false;
    }

    private void UpdateHighlight()
    {
        if (_highlightImage == null) return;
        _highlightImage.enabled = _selectedToggle != null && _selectedToggle.IsInteractable() &&
            EventSystem.current != null && EventSystem.current.currentSelectedGameObject == _selectedToggle.gameObject;
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

        UpdateHighlight();
    }


    public void Selection(int index)
    {
        Selection($"素材 {index}");
    }

    public void Selection(string label)
    {
        SetToggleWithoutEvent(true);

        if (_selectedPlane != null)
        {
            _selectedPlane.SetActive(true);
        }

        if (_selectedText != null)
        {
            _selectedText.text = label;
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
